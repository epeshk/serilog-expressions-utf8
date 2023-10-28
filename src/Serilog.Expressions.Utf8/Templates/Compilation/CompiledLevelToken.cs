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

using Serilog.Events;
using Serilog.Expressions;
using Serilog.Parsing;
using Serilog.Templates.Rendering;
using Serilog.Templates.Themes;
using Serilog.Utf8.Commons;

namespace Serilog.Templates.Compilation;

class CompiledLevelToken : CompiledTemplate
{
    readonly string? _format;
    readonly Alignment _alignment;
    readonly Style[] _levelStyles;
    private readonly byte[][] _renderedLevels;

    public CompiledLevelToken(string? format, Alignment? alignment, Utf8TemplateTheme theme)
    {
        _format = format;
        _alignment = alignment ?? new Alignment(AlignmentDirection.Left, 0);
        _levelStyles = new[]
        {
            theme.GetStyle(Utf8TemplateThemeStyle.LevelVerbose),
            theme.GetStyle(Utf8TemplateThemeStyle.LevelDebug),
            theme.GetStyle(Utf8TemplateThemeStyle.LevelInformation),
            theme.GetStyle(Utf8TemplateThemeStyle.LevelWarning),
            theme.GetStyle(Utf8TemplateThemeStyle.LevelError),
            theme.GetStyle(Utf8TemplateThemeStyle.LevelFatal),
        };
        _renderedLevels = new[]
        {
            LevelRenderer.GetLevelMoniker(LogEventLevel.Verbose, _format),
            LevelRenderer.GetLevelMoniker(LogEventLevel.Debug, _format),
            LevelRenderer.GetLevelMoniker(LogEventLevel.Information, _format),
            LevelRenderer.GetLevelMoniker(LogEventLevel.Warning, _format),
            LevelRenderer.GetLevelMoniker(LogEventLevel.Error, _format),
            LevelRenderer.GetLevelMoniker(LogEventLevel.Fatal, _format)
        };
    }

    public override void Evaluate(EvaluationContext ctx, ref Utf8Writer output)
    {
        var invisibleCharacterCount = 0;
        
        EvaluateInternal(ctx, ref output, ref invisibleCharacterCount, _alignment);
    }

    void EvaluateInternal(EvaluationContext ctx, ref Utf8Writer output, ref int invisibleCharacterCount, Alignment alignment)
    {
        var levelIndex = (int) ctx.LogEvent.Level;
        if (levelIndex < 0 || levelIndex >= _levelStyles.Length)
            return;

        var style = _levelStyles[levelIndex];

        var renderedLevel = levelIndex < _renderedLevels.Length
            ? _renderedLevels[levelIndex]
            : LevelRenderer.GetLevelMoniker(ctx.LogEvent.Level, _format);

        style.Set(ref output, ref invisibleCharacterCount);
        // 99.999% log level is ASCII-only
        Padding.ApplyAscii(ref output, renderedLevel, alignment);
        style.Reset(ref output);
    }
}