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
        eto.Inner = eto.ShaderExpression;
        toparse = """
shader MyShader
{
    void MyMethod()
    {
        int a = 0;
        int b = (a - 10 / 3 ) * 32 +( 8 % streams.color.Normalize() + 2 << 5);
    }
}
""";
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
        var expression = Grammar.Match<ShaderClassParsers, SDSL.AST.ShaderClass>(toparse);
    }
}