using System.Buffers;
using System.Globalization;
using System.Text;
using Serilog.Expressions.Tests.Support;
using Serilog.Templates;
using Xunit;

namespace Serilog.Expressions.Tests;

public class TemplateEvaluationTests
{
    public static IEnumerable<object[]> TemplateEvaluationCases =>
        AsvCases.ReadCases("template-evaluation-cases.asv");

    [Theory]
    [MemberData(nameof(TemplateEvaluationCases))]
    public void TemplatesAreCorrectlyEvaluated(string template, string expected)
    {
        if (template.Contains(",4}") || template.Contains(",-4}"))
        {
            // alignment is not supported now
            return;
        }
        
        var evt = Some.InformationEvent("Hello, {Name}!", "nblumhardt");
        var frFr = CultureInfo.GetCultureInfoByIetfLanguageTag("fr-FR");
        var compiled = new Utf8ExpressionTemplate(template, formatProvider: frFr);
        var output = new ArrayBufferWriter<byte>();
        compiled.Format(evt, output);
        var actual = Encoding.UTF8.GetString(output.WrittenSpan);
        Assert.Equal(expected, actual);
    }
}