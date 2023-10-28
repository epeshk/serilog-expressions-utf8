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
using Serilog.Utf8.Commons;

namespace Serilog.Templates.Themes;

readonly struct Style
{
    readonly byte[]? _ansiStyle;

    public Style(byte[] ansiStyle)
    {
        _ansiStyle = ansiStyle;
    }

    internal void Set(ref Utf8Writer output, ref int invisibleCharacterCount)
    {
        if (_ansiStyle != null)
        {
            output.Write(_ansiStyle);
            invisibleCharacterCount += _ansiStyle.Length;
            invisibleCharacterCount += "\x1b[0m"u8.Length;
        }

    }

    internal void Reset(ref Utf8Writer output)
    {
        if (_ansiStyle != null)
            output.Write("\x1b[0m"u8);
    }
}

static class Utf8WriterStyleExtensions
{
    public static void Write(this ref Utf8Writer writer, Style style, scoped ReadOnlySpan<byte> bytes, ref int inv)
    {
        style.Set(ref writer, ref inv);
        writer.Write(bytes);
        style.Reset(ref writer);
    }

    public static void Write(this ref Utf8Writer writer, Style style, scoped ReadOnlySpan<byte> a, scoped ReadOnlySpan<byte> b, ref int inv)
    {
        style.Set(ref writer, ref inv);
        writer.Write(a, b);
        style.Reset(ref writer);
    }

    public static void Write(this ref Utf8Writer writer, Style style, scoped ReadOnlySpan<byte> a, scoped ReadOnlySpan<byte> b, scoped ReadOnlySpan<byte> c, ref int inv)
    {
        style.Set(ref writer, ref inv);
        writer.Write(a, b, c);
        style.Reset(ref writer);
    }

    public static void Write(this ref Utf8Writer writer, Style style, byte a, scoped ReadOnlySpan<byte> bytes, byte b, ref int inv)
    {
        style.Set(ref writer, ref inv);
        writer.Write(a, bytes, b);
        style.Reset(ref writer);
    }

    public static void WriteChars(this ref Utf8Writer writer, Style style, scoped ReadOnlySpan<char> s, ref int inv)
    {
        style.Set(ref writer, ref inv);
        writer.WriteChars(s);
        style.Reset(ref writer);
    }

    public static void Write(this ref Utf8Writer writer, Style style, byte value, ref int inv)
    {
        style.Set(ref writer, ref inv);
        writer.Write(value);
        style.Reset(ref writer);
    }

#if NET8_0_OR_GREATER
    public static void Format<TFormattable>(this ref Utf8Writer writer, Style style, TFormattable formattable, ReadOnlySpan<char> format, IFormatProvider? provider, ref int inv)
        where TFormattable : IUtf8SpanFormattable
    {
        style.Set(ref writer, ref inv);
        writer.Format(formattable, format, provider);
        style.Reset(ref writer);
    }
#endif
}