using System.Collections.Generic;
using Serilog.Expressions.Compilation;
using Serilog.Expressions.Parsing;
using Serilog.Expressions.Tests.Support;
using Xunit;

namespace Serilog.Expressions.Tests
{
    public class ExpressionTranslationTests
    {
        public static IEnumerable<object[]> ExpressionEvaluationCases =>
            AsvCases.ReadCases("translation-cases.asv");

        [Theory]
        [MemberData(nameof(ExpressionEvaluationCases))]
        public void ExpressionsAreCorrectlyTranslated(string expr, string expected)
        {
            var parsed = new ExpressionParser().Parse(expr);
            var translated = ExpressionCompiler.Translate(parsed);
            var actual = translated.ToString();
            Assert.Equal(expected, actual);
        }
    }
}
