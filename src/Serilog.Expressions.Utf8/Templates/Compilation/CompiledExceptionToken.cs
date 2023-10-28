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

using Serilog.Expressions;
using Serilog.Templates.Themes;
using Serilog.Utf8.Commons;

namespace Serilog.Templates.Compilation;

class CompiledExceptionToken : CompiledTemplate
{
    const string StackFrameLinePrefix = "   ";

    readonly Style _text, _secondaryText;

    public CompiledExceptionToken(Utf8TemplateTheme theme)
    {
        _text = theme.GetStyle(Utf8TemplateThemeStyle.Text);
        _secondaryText = theme.GetStyle(Utf8TemplateThemeStyle.SecondaryText);
    }

    public override void Evaluate(EvaluationContext ctx, ref Utf8Writer output)
    {
        // Padding and alignment are not applied by this renderer.

        if (ctx.LogEvent.Exception is null)
            return;

        var lines = new StringReader(ctx.LogEvent.Exception.ToString());
        while (lines.ReadLine() is { } nextLine)
        {
            var style = nextLine.StartsWith(StackFrameLinePrefix) ? _secondaryText : _text;
            var _ = 0;
            style.Set(ref output, ref _);
            output.WriteChars(nextLine);
            output.WriteNewLine();
            style.Reset(ref output);
        }
    }
}