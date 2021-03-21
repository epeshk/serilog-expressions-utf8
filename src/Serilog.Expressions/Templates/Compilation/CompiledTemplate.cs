using System;
using System.IO;
using Serilog.Events;
using Serilog.Expressions;

namespace Serilog.Templates.Compilation
{
    abstract class CompiledTemplate
    {
        public abstract void Evaluate(EvaluationContext ctx, TextWriter output, IFormatProvider? formatProvider);
    }
}