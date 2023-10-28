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

using System.Globalization;
using System.Runtime.InteropServices;
using Serilog.Events;
using Serilog.Expressions;
using Serilog.Parsing;
using Serilog.Templates.Rendering;
using Serilog.Templates.Themes;
using Serilog.Utf8.Commons;

namespace Serilog.Templates.Compilation;

class CompiledFormattedExpression : CompiledTemplate
{
    readonly ThemedJsonValueFormatter _jsonFormatter;
    readonly Evaluatable _expression;
    readonly string? _format;
    readonly Alignment? _alignment;
    readonly IFormatProvider? _formatProvider;
    readonly Style _secondaryText;

    public CompiledFormattedExpression(Evaluatable expression, string? format, Alignment? alignment, IFormatProvider? formatProvider, Utf8TemplateTheme theme)
    {
        _expression = expression ?? throw new ArgumentNullException(nameof(expression));
        _format = format;
        _alignment = alignment;
        _formatProvider = formatProvider;
        _secondaryText = theme.GetStyle(Utf8TemplateThemeStyle.SecondaryText);
        _jsonFormatter = new(theme);
    }

    public override void Evaluate(EvaluationContext ctx, ref Utf8Writer output)
    {
        var invisibleCharacterCount = 0;

        EvaluateUnaligned(ctx, ref output, _formatProvider, ref invisibleCharacterCount);
    }

    void EvaluateUnaligned(EvaluationContext ctx, ref Utf8Writer output, IFormatProvider? formatProvider, ref int invisibleCharacterCount)
    {
        var value = _expression(ctx);
        if (value == null)
            return; // Undefined is empty

        if (value is ScalarValue scalar)
        {
            if (scalar.Value is null)
                return; // Null is empty
#if NET8_0_OR_GREATER
            _secondaryText.Set(ref output, ref invisibleCharacterCount);
            if (scalar.Value is IUtf8SpanFormattable u8sf)
            {
                output.Format(u8sf, _format, formatProvider);
            }
            else
#endif
            {
                if (scalar.Value is ISpanFormattable sf && output.TryFormat(sf, _format, _formatProvider))
                {
                }
                else
                {
                    var str = scalar.Value is IFormattable fmt
                        ? fmt.ToString(_format, formatProvider)
                        : scalar.Value.ToString();
                    output.WriteChars(str);
                }
            }

            _secondaryText.Reset(ref output);
        }
        else
        {
            invisibleCharacterCount += _jsonFormatter.Format(value, ref output);
        }
    }
}