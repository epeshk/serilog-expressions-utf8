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
using Serilog.Expressions;
using Serilog.Templates.Themes;
using Serilog.Utf8.Commons;

namespace Serilog.Templates.Compilation;

class CompiledLiteralText : CompiledTemplate
{
    readonly byte[] _text;
    readonly Style _style;

    public CompiledLiteralText(string text, Utf8TemplateTheme theme)
    {
        ArgumentNullException.ThrowIfNull(text);
        _text = System.Text.Encoding.UTF8.GetBytes(text);
        _style = theme.GetStyle(Utf8TemplateThemeStyle.TertiaryText);
    }

    public override void Evaluate(EvaluationContext ctx, ref Utf8Writer output)
    {
        int _ = 0;
        output.Write(_style, _text, ref _);
    }
//
//     public override void Append(StringBuilder builder)
//     {
//         if (_text.Length == 0)
//             return;
//         if (_text.Length == 1)
//         {
//             var ch = _text[0];
//             if (char.IsAscii(ch))
//                 builder.Append(
// $"""
// if (!builder.TryAppend((byte){(byte)ch}))
//   return false;
// """);
//             else
//                 builder.Append(
// $"""
//  if (!builder.TryAppendChar((char){(int)ch}))
//    return false;
//  """);
//         }
//
//         builder.Append(""""
//
//
//
//                        """");
//     }
}