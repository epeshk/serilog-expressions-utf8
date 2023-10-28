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

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Serilog.Events;
using Serilog.Expressions;
using Serilog.Formatting;
using Serilog.Templates.Compilation;
using Serilog.Templates.Compilation.NameResolution;
using Serilog.Templates.Parsing;
using Serilog.Templates.Themes;
using Serilog.Utf8.Commons;

namespace Serilog.Templates;

/// <summary>
/// Formats <see cref="LogEvent"/>s into text using embedded expressions.
/// </summary>
public class Utf8ExpressionTemplate : IBufferWriterFormatter
{
    readonly CompiledTemplate _compiled;

    /// <summary>
    /// Construct an <see cref="Utf8ExpressionTemplate"/>.
    /// </summary>
    /// <param name="template">The template text.</param>
    /// <param name="result">The parsed template, if successful.</param>
    /// <param name="error">A description of the error, if unsuccessful.</param>
    /// <returns><c langword="true">true</c> if the template was well-formed.</returns>
    public static bool TryParse(
        string template,
        [MaybeNullWhen(false)] out Utf8ExpressionTemplate result,
        [MaybeNullWhen(true)] out string error)
    {
        if (template == null) throw new ArgumentNullException(nameof(template));
        return TryParse(template, null, null, null, false, out result, out error);
    }

    /// <summary>
    /// Construct an <see cref="Utf8ExpressionTemplate"/>.
    /// </summary>
    /// <param name="template">The template text.</param>
    /// <param name="formatProvider">Optionally, an <see cref="IFormatProvider"/> to use when formatting
    /// embedded values.</param>
    /// <param name="theme">Optionally, an ANSI theme to apply to the template output.</param>
    /// <param name="result">The parsed template, if successful.</param>
    /// <param name="error">A description of the error, if unsuccessful.</param>
    /// <param name="nameResolver">Optionally, a <see cref="NameResolver"/>
    /// with which to resolve function names that appear in the template.</param>
    /// <param name="applyThemeWhenOutputIsRedirected">Apply <paramref name="theme"/> even when
    /// <see cref="System.Console.IsOutputRedirected"/> or <see cref="Console.IsErrorRedirected"/> returns <c>true</c>.</param>
    /// <returns><c langword="true">true</c> if the template was well-formed.</returns>
    public static bool TryParse(
        string template,
        IFormatProvider? formatProvider,
        NameResolver? nameResolver,
        Utf8TemplateTheme? theme,
        bool applyThemeWhenOutputIsRedirected,
        [MaybeNullWhen(false)] out Utf8ExpressionTemplate result,
        [MaybeNullWhen(true)] out string error)
    {
        if (template == null) throw new ArgumentNullException(nameof(template));

        var templateParser = new TemplateParser();
        if (!templateParser.TryParse(template, out var parsed, out error))
        {
            result = null;
            return false;
        }

        var planned = TemplateLocalNameBinder.BindLocalValueNames(parsed);

        result = new(
            TemplateCompiler.Compile(
                planned,
                formatProvider,
                TemplateFunctionNameResolver.Build(nameResolver, planned),
                SelectTheme(theme, applyThemeWhenOutputIsRedirected)));

        return true;
    }

    Utf8ExpressionTemplate(CompiledTemplate compiled)
    {
        _compiled = compiled;
    }

    /// <summary>
    /// Construct an <see cref="ExpressionTemplate"/>.
    /// </summary>
    /// <param name="template">The template text.</param>
    /// <param name="formatProvider">Optionally, an <see cref="IFormatProvider"/> to use when formatting
    /// embedded values.</param>
    /// <param name="nameResolver">Optionally, a <see cref="NameResolver"/>
    /// with which to resolve function names that appear in the template.</param>
    /// <param name="theme">Optionally, an ANSI theme to apply to the template output.</param>
    /// <param name="applyThemeWhenOutputIsRedirected">Apply <paramref name="theme"/> even when
    /// <see cref="Console.IsOutputRedirected"/> or <see cref="Console.IsErrorRedirected"/> returns <c>true</c>.</param>
    public Utf8ExpressionTemplate(
        string template,
        IFormatProvider? formatProvider = null,
        NameResolver? nameResolver = null,
        Utf8TemplateTheme? theme = null,
        bool applyThemeWhenOutputIsRedirected = false)
    {
        if (template == null) throw new ArgumentNullException(nameof(template));

        var templateParser = new TemplateParser();
        if (!templateParser.TryParse(template, out var parsed, out var error))
            throw new ArgumentException(error);

        var planned = TemplateLocalNameBinder.BindLocalValueNames(parsed);

        _compiled = TemplateCompiler.Compile(
            planned,
            formatProvider,
            TemplateFunctionNameResolver.Build(nameResolver, planned),
            SelectTheme(theme, applyThemeWhenOutputIsRedirected));
    }

    static Utf8TemplateTheme SelectTheme(Utf8TemplateTheme? supplied, bool applyThemeWhenOutputIsRedirected)
    {
        if (supplied == null ||
            (Console.IsOutputRedirected || Console.IsErrorRedirected) && !applyThemeWhenOutputIsRedirected)
        {
            return Utf8TemplateTheme.None;
        }

        return supplied;
    }

    /// <inheritdoc />
    public void Format(LogEvent logEvent, IBufferWriter<byte> buffer)
    {
        var utf8Writer = new Utf8Writer(buffer);
        _compiled.Evaluate(new(logEvent), ref utf8Writer);
        utf8Writer.Flush();
    }

    public Encoding Encoding { get; } = Encoding.UTF8;
}