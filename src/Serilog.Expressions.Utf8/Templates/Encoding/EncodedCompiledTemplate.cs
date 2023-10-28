// using Serilog.Expressions;
// using Serilog.Templates.Compilation;
//
// namespace Serilog.Templates.Encoding
// {
//     class EncodedCompiledTemplate : CompiledTemplate
//     {
//         readonly CompiledTemplate _inner;
//         readonly TemplateOutputEncoder _encoder;
//
//         public EncodedCompiledTemplate(CompiledTemplate inner, TemplateOutputEncoder encoder)
//         {
//             _inner = inner;
//             _encoder = encoder;
//         }
//
//         public override void Evaluate(EvaluationContext ctx, ref Utf8Writer output)
//         {
//             _inner.Evaluate(ctx, ref output);
//         }
//     }
// }