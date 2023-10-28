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
using Serilog.Events;
using Serilog.Utf8.Commons;

// ReSharper disable ForCanBeConvertedToForeach

namespace Serilog.Templates.Themes;

class ThemedJsonValueFormatter : SpanLogEventPropertyValueVisitor<int>
{
    const string TypeTagPropertyName = "$type";

    readonly Style _null, _bool, _num, _string, _scalar, _tertiary, _name;

    public ThemedJsonValueFormatter(Utf8TemplateTheme theme)
    {
        _null = theme.GetStyle(Utf8TemplateThemeStyle.Null);
        _bool = theme.GetStyle(Utf8TemplateThemeStyle.Boolean);
        _num = theme.GetStyle(Utf8TemplateThemeStyle.Number);
        _string = theme.GetStyle(Utf8TemplateThemeStyle.String);
        _scalar = theme.GetStyle(Utf8TemplateThemeStyle.Scalar);
        _tertiary = theme.GetStyle(Utf8TemplateThemeStyle.TertiaryText);
        _name = theme.GetStyle(Utf8TemplateThemeStyle.Name);
    }

    public int Format(LogEventPropertyValue value, ref Utf8Writer output)
    {
        return Visit(ref output, value);
    }

    protected override int VisitScalarValue(ref Utf8Writer state, ScalarValue scalar)
    {
        return FormatLiteralValue(scalar, ref state);
    }

    protected override int VisitSequenceValue(ref Utf8Writer state, SequenceValue sequence)
    {
        var count = 0;

        state.Write(_tertiary, (byte)'[', ref count);

        byte delim = 0;
        for (var index = 0; index < sequence.Elements.Count; ++index)
        {
            if (delim != 0)
                state.Write(_tertiary, delim, ref count);

            delim = (byte)',';
            count += Visit(ref state, sequence.Elements[index]);
        }

        state.Write(_tertiary, (byte)']', ref count);

        return count;
    }

    protected override int VisitStructureValue(ref Utf8Writer state, StructureValue structure)
    {
        var count = 0;

        state.Write(_tertiary, (byte)'{', ref count);

        byte delim = 0;
        for (var index = 0; index < structure.Properties.Count; ++index)
        {
            if (delim != 0)
                state.Write(_tertiary, delim, ref count);

            delim = (byte)',';

            var property = structure.Properties[index];

            _name.Set(ref state, ref count);
            WriteQuotedJsonString(property.Name, ref state);
            _name.Reset(ref state);

            state.Write(_tertiary, (byte)':', ref count);

            count += Visit(ref state, property.Value);
        }

        if (structure.TypeTag != null)
        {
            state.Write(_tertiary, delim, ref count);

            _name.Set(ref state, ref count);
            WriteQuotedJsonString(TypeTagPropertyName, ref state);
            _name.Reset(ref state);

            state.Write(_tertiary, (byte)':', ref count);

            _string.Set(ref state, ref count);
            WriteQuotedJsonString(structure.TypeTag, ref state);
            _string.Reset(ref state);
        }

        state.Write(_tertiary, (byte)'}', ref count);

        return count;
    }

    protected override int VisitDictionaryValue(ref Utf8Writer state, DictionaryValue dictionary)
    {
        var count = 0;

        state.Write(_tertiary, (byte)'{', ref count);

        byte delim = 0;
        foreach (var element in dictionary.Elements)
        {
            if (delim != 0)
                state.Write(_tertiary, delim, ref count);

            delim = (byte)',';

            var style = element.Key.Value == null
                ? _null
                : element.Key.Value is string
                    ? _string
                    : _scalar;

            style.Set(ref state, ref count);
            WriteQuotedJsonString(element.Key.Value?.ToString() ?? "null", ref state);
            style.Reset(ref state);
                
            state.Write(_tertiary, (byte)':', ref count);

            count += Visit(ref state, element.Value);
        }

        state.Write(_tertiary, (byte)'}', ref count);

        return count;
    }

    int FormatLiteralValue(ScalarValue scalar, ref Utf8Writer output)
    {
        var value = scalar.Value;
        var count = 0;

        if (value == null)
        {
            output.Write(_null, "null"u8, ref count);
            return count;
        }

        if (value is string str)
        {
            _string.Set(ref output, ref count);
            WriteQuotedJsonString(str, ref output);
            _string.Reset(ref output);
            return count;
        }
        
        if (value is char ch)
        {
            _scalar.Set(ref output, ref count);
            WriteQuotedJsonString(ch.ToString(), ref output);
            _scalar.Reset(ref output);
            return count;
        }

        // if (value is DateTimeOffset dto)
        // {
        //     _scalar.Set(ref output, ref count);
        //     WriteQuotedJsonString(dto.ToString(CultureInfo.InvariantCulture), ref output);
        //     _scalar.Reset(ref output);
        //     return count;
        // }
        var formatStr = value is DateTimeOffset || value is DateTime ? "O" : null;
#if NET8_0_OR_GREATER
        if (value is IUtf8SpanFormattable u8sf)
        {
            var theme = value switch
            {
                bool => _bool,
                int or uint or long or ulong or decimal or byte or sbyte or short or ushort or double or float => _num,
                _ => _scalar
            };

            var isPrimitive = u8sf.GetType().IsPrimitive || u8sf is decimal;
            if (!isPrimitive)
                output.Write((byte)'\"');
            
            output.Format(u8sf, formatStr, CultureInfo.InvariantCulture);
            if (!isPrimitive)
                output.Write((byte)'\"');

            // var format = value is DateTimeOffset ? "O" : ReadOnlySpan<char>.Empty;
            // output.Format(theme, u8sf, ReadOnlySpan<char>.Empty, CultureInfo.InvariantCulture, ref count);
            return count;
        }
#endif

        if (value is ISpanFormattable sf && (sf.GetType().IsPrimitive || sf is decimal))
        {
            var theme = value switch
            {
                bool => _bool,
                int or uint or long or ulong or decimal or byte or sbyte or short or ushort or double or float => _num,
                _ => _scalar
            };


            theme.Set(ref output, ref count);
            output.TryFormat(sf, formatStr, CultureInfo.InvariantCulture);
            theme.Reset(ref output);

            return count;
        }
        
        var strValue = value is IFormattable f
            ? f.ToString(formatStr, CultureInfo.InvariantCulture)
            : value.ToString();

        _scalar.Set(ref output, ref count);
        WriteQuotedJsonString(strValue ?? "", ref output);
        _scalar.Reset(ref output);

        return count;
    }
    
    public static void WriteQuotedJsonString(ReadOnlySpan<char> str, ref Utf8Writer output)
    {
        output.Write((byte)'\"');

        var cleanSegmentStart = 0;
        var anyEscaped = false;

        for (var i = 0; i < str.Length; ++i)
        {
            var c = str[i];
            if (c is < (char)32 or '\\' or '"')
            {
                anyEscaped = true;

                output.WriteChars(str.Slice(cleanSegmentStart, i - cleanSegmentStart));

                cleanSegmentStart = i + 1;

                var s = c switch
                {
                    '"' => "\\\""u8,
                    '\\' => @"\\"u8,
                    '\n' => "\\n"u8,
                    '\r' => "\\r"u8,
                    '\f' => "\\f"u8,
                    '\t' => "\\t"u8,
                    _ => "\\u"u8
                };
                
                output.Write(s);

                if (s[1] == 'u') output.WriteChars(((int)c).ToString("X4")); //TODO: remove tostring
            }
        }

        if (anyEscaped)
        {
            if (cleanSegmentStart != str.Length)
                output.WriteChars(str.Slice(cleanSegmentStart));
        }
        else
        {
            output.WriteChars(str);
        }

        output.Write((byte)'\"');
    }
}