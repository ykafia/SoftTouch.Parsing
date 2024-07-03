using BenchmarkDotNet.Attributes;
using SoftTouch.Parsing.SDSL;

namespace SoftTouch.Parsing.Benchmarks;
using StrideEtoParser = global::SDSL.Parsing.ShaderMixinParser;

[MemoryDiagnoser]
public class ParserBench
{
    string toparse;

    public ParserBench()
    {
        toparse = """
namespace machin
{
    shader Parent 
    {
        stream int a;

        void Method()
        {
            int a = 0;
            float4 buffer = float4(1,3, float2(1,2));
            float4x4 a = float4x4(
                float4(1,2,3,4),
                float4(1,2,3,4),
                float4(1,2,3,4),
                float4(1,2,3,4)
            );
            int b = (a - 10 / 3 ) * 32 +( streams.color.Normalize() + 2);
            if(a == 2)
            {
            }
        }
    };
}
""";
        // Grammar.Match<PostfixParser, Expression>("machin.chose[3].something.else[3]");
    }

    [Benchmark]
    public void SoftTouchParse()
    {
        var match = Grammar.Match<ShaderFileParser, SDSL.AST.ShaderFile>(toparse);
        if (match.Errors.Count > 0)
            throw new Exception(string.Join("\n", match.Errors.Select(x => x.ToString())));
    }
    
    [Benchmark]
    public void StrideEtoParse()
    {
        var match = StrideEtoParser.ParseShader(toparse);
    }
    
    [Benchmark]
    public void StrideParse()
    {
        var match = Stride.Shaders.Parser.StrideShaderParser.Parse(toparse, "myfile");
    }
}