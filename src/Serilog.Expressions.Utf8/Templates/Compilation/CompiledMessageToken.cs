// Copyright © Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Text;
using Serilog.Events;
using Serilog.Expressions;
using Serilog.Parsing;
using Serilog.Templates.Rendering;
using Serilog.Templates.Themes;
using Serilog.Utf8.Commons;

namespace Serilog.Templates.Compilation;

class CompiledMessageToken : CompiledTemplate
{
    readonly IFormatProvider? _formatProvider;
    readonly Alignment? _alignment;
    readonly Style _text, _invalid, _null, _bool, _string, _num, _scalar;
    readonly ThemedJsonValueFormatter _jsonFormatter;

    public CompiledMessageToken(IFormatProvider? formatProvider, Alignment? alignment, Utf8TemplateTheme theme)
    {
        _formatProvider = formatProvider;
        _alignment = alignment;
        _text = theme.GetStyle(Utf8TemplateThemeStyle.Text);
        _null = theme.GetStyle(Utf8TemplateThemeStyle.Null);
        _bool = theme.GetStyle(Utf8TemplateThemeStyle.Boolean);
        _num = theme.GetStyle(Utf8TemplateThemeStyle.Number);
        _string = theme.GetStyle(Utf8TemplateThemeStyle.String);
        _scalar = theme.GetStyle(Utf8TemplateThemeStyle.Scalar);
        _invalid = theme.GetStyle(Utf8TemplateThemeStyle.Invalid);
        _jsonFormatter = new(theme);
    }

    public override void Evaluate(EvaluationContext ctx, ref Utf8Writer output)
    {
        var invisibleCharacterCount = 0;

        EvaluateUnaligned(ctx, ref output, ref invisibleCharacterCount);
    }

    void EvaluateUnaligned(EvaluationContext ctx, ref Utf8Writer output, ref int invisibleCharacterCount)
    {
        var template = Utf8MessageTemplateCache.Get(ctx.LogEvent.MessageTemplate);
        foreach (var token in template.Tokens)
        {
            switch (token)
            {
                case Utf8TextToken tt:
                {
                    _text.Set(ref output, ref invisibleCharacterCount);
                    output.Write(tt.AsSpan());
                    _text.Reset(ref output);
                    break;
                }
                case Utf8PropertyToken pt:
                {
                    EvaluateProperty(ctx.LogEvent.Properties, pt, ref output, ref invisibleCharacterCount);
                    break;
                }
                default:
                {
                    output.WriteChars(token.ToString());
                    break;
                }
            }
        }
    }

    void EvaluateProperty(IReadOnlyDictionary<string,LogEventPropertyValue> properties, Utf8PropertyToken pt, ref Utf8Writer output, ref int invisibleCharacterCount)
    {
        if (!properties.TryGetValue(pt.PropertyName, out var value))
        {
            output.WriteChars(_invalid, pt.ToString(), ref invisibleCharacterCount);
            return;
        }

        EvaluatePropertyUnaligned(value, ref output, pt.Format, pt.IsCached, ref invisibleCharacterCount);
    }

    void EvaluatePropertyUnaligned(LogEventPropertyValue propertyValue, ref Utf8Writer output, string? format, bool cacheStrings, ref int invisibleCharacterCount)
    {
        if (propertyValue is not ScalarValue scalar)
        {
            invisibleCharacterCount += _jsonFormatter.Format(propertyValue, ref output);
            return;
        }

        var value = scalar.Value;

        if (value == null)
        {
            output.Write(_null, "null"u8, ref invisibleCharacterCount);
            return;
        }

        if (value is string str)
        {
            if (cacheStrings)
                output.Write(_string, Utf8StringCache.Get(str), ref invisibleCharacterCount);
            else
                output.WriteChars(_string, str, ref invisibleCharacterCount);

            return;
        }

#if NET8_0_OR_GREATER
        if (value is IUtf8SpanFormattable u8sf)
        {
            var theme = value switch
            {
                bool => _bool,
                int or uint or long or ulong or decimal or byte or sbyte or short or ushort or double or float => _num,
                _ => _scalar
            };

            int inv = 0;
            output.Format(theme, u8sf, format, _formatProvider, ref inv);
            invisibleCharacterCount += inv;
            return;
        }
#endif
        if (value is ISpanFormattable sf && output.TryFormat(sf, format, _formatProvider))
            return;

        var strVal = value is IFormattable formattable
            ? formattable.ToString(format, _formatProvider)
            : value.ToString();

        output.WriteChars(_scalar, strVal, ref invisibleCharacterCount);
    }

    void EvaluatePropertyAlignedInternal(LogEventPropertyValue propertyValue, ref Utf8Writer output, string? format, bool cacheStrings, ref int invisibleCharacterCount, ref int chars)
    {
        if (propertyValue is not ScalarValue scalar)
        {
            invisibleCharacterCount += _jsonFormatter.Format(propertyValue, ref output);
            return;
        }

        var value = scalar.Value;

        if (value == null)
        {
            chars += 4;
            output.Write(_null, "null"u8, ref invisibleCharacterCount);
            return;
        }

        if (value is string str)
        {
            chars += str.Length;
            if (cacheStrings)
                output.Write(_string, Utf8StringCache.Get(str), ref invisibleCharacterCount);
            else
                output.WriteChars(_string, str, ref invisibleCharacterCount);

            return;
        }

#if NET8_0_OR_GREATER
        if (value is IUtf8SpanFormattable u8sf)
        {
            var theme = value switch
            {
                bool => _bool,
                int or uint or long or ulong or decimal or byte or sbyte or short or ushort or double or float => _num,
                _ => _scalar
            };

            int inv = 0;
            output.Format(theme, u8sf, format, _formatProvider, ref inv);
            invisibleCharacterCount += inv;
            return;
        }
#endif

        if (value is ISpanFormattable sf && TryWriteSpanFormattable(ref output, sf, format, ref invisibleCharacterCount))
            return;

        var strVal = value is IFormattable formattable
            ? formattable.ToString(format, _formatProvider)
            : value.ToString();

        chars += strVal?.Length ?? 0;
        output.WriteChars(_scalar, strVal, ref invisibleCharacterCount);
    }

    bool TryWriteSpanFormattable(ref Utf8Writer output, ISpanFormattable formattable, string? format, ref int invisibleCharacterCount)
    {
        var theme = formattable switch
        {
            int or uint or long or ulong or decimal or byte or sbyte or short or ushort or double or float => _num,
            _ => _scalar
        };

        int inv = 0;
        theme.Set(ref output, ref inv);
        try
        {
            if (output.TryFormat(formattable, format, _formatProvider))
                return true;
        }
        finally
        {
            theme.Reset(ref output);
            invisibleCharacterCount += inv;
        }

        return false;
    }
}