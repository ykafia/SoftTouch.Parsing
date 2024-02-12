using BenchmarkDotNet.Attributes;
using SDSL.Parsing.AST.Shader;
using SDSL.Parsing.Grammars.SDSL;
using SoftTouch.Parsing.SDSL;

namespace SoftTouch.Parsing.Benchmarks;


[MemoryDiagnoser]
public class ParserBench
{
    SDSLGrammar eto;
    string toparse;

    public ParserBench()
    {
        eto = new();
        eto.Inner = eto.PrimaryExpression;
        toparse = "machin.chose[3].something.else[3]";
        // Grammar.Match<PostfixParser, Expression>("machin.chose[3].something.else[3]");
    }

    [Benchmark]
    public void StrideParse()
    {
        var match = eto.Match(toparse);
        var token = ShaderToken.Tokenize(match);
    }

    [Benchmark]
    public void SoftTouchParse()
    {
        var expression = Grammar.Match<ExpressionParser, SDSL.AST.Expression>(toparse);
    }
}