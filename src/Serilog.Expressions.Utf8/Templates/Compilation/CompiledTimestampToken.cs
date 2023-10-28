using System.Runtime.CompilerServices;
using Serilog.Expressions;
using Serilog.Parsing;
using Serilog.Templates.Rendering;
using Serilog.Templates.Themes;
using Serilog.Utf8.Commons;

namespace Serilog.Templates.Compilation;

class CompiledTimestampToken : CompiledTemplate
{
  readonly string? _format;
  readonly Alignment? _alignment;
  readonly IFormatProvider? _formatProvider;
  readonly Style _secondaryText;
  readonly TimestampFormatter _formatter;

  public CompiledTimestampToken(string? format, Alignment? alignment, IFormatProvider? formatProvider, Utf8TemplateTheme theme)
  {
    _format = format;
    _alignment = alignment ?? new Alignment(AlignmentDirection.Left, 0);
    _formatProvider = formatProvider;
    _secondaryText = theme.GetStyle(Utf8TemplateThemeStyle.SecondaryText);

    _formatter = new TimestampFormatter(format ?? "O");
  }

  public override void Evaluate(EvaluationContext ctx, ref Utf8Writer output)
  {
    int _ = 0;
    var timestamp = ctx.LogEvent.Timestamp;
    _secondaryText.Set(ref output, ref _);
    Evaluate(timestamp, ref output);
    _secondaryText.Reset(ref output);
  }

  void Evaluate(DateTimeOffset timestamp, ref Utf8Writer output)
  {
    output.Reserve(64);
    if (_formatter.TryFormat(timestamp, output.Span, out var bytesWritten))
      output.Advance(bytesWritten);
    else
      Retry(timestamp, output);
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  void Retry(DateTimeOffset timestamp, Utf8Writer output)
  {
      output.Reserve(64);
      if (_formatter.TryFormat(timestamp, output.Span, out int bytesWritten))
          output.Advance(bytesWritten);
      else
          output.WriteChars("<ERROR>");
  }
}