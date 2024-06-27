using CommunityToolkit.HighPerformance;

namespace SoftTouch.Parsing.SDSL;

public class ReadOnlyMemoryCharComparer : IEqualityComparer<ReadOnlyMemory<char>>
{
    public static ReadOnlyMemoryCharComparer Instance { get; } = new();

    public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y) =>
        x.Span.Equals(y.Span, StringComparison.OrdinalIgnoreCase);

    public int GetHashCode(ReadOnlyMemory<char> obj) =>
        string.GetHashCode(obj.Span, StringComparison.OrdinalIgnoreCase);
}


public static class Reserved
{
    internal static readonly HashSet<string> TypeNames;
    internal static readonly HashSet<string> Keywords;

    static Reserved()
    {
        TypeNames = new();
        List<string> types = [
            "float",
            "half",
            "byte",
            "sbyte",
            "short",
            "ushort",
            "int",
            "uint",
            "long",
            "double",
            "bool",
            "min10float",
            "min16float",
            "min12int",
            "min16int",
            "min16uint"
        ];

        foreach(var t in types)
        {
            for(int i = 1; i < 5; i ++)
            {
                TypeNames.Add($"{t}{i}");
                for (int j = 1; j < 5; j++)
                    TypeNames.Add($"{t}{i}x{j}");
            }
        }


        Keywords = [
            ..TypeNames,
            "AppendStructuredBuffer",
            "asm",
            "asm_fragment",
            "BlendState",
            "bool",
            "break",
            "Buffer",
            "ByteAddressBuffer",
            "case",
            "cbuffer",
            "centroid",
            "class",
            "column_major",
            "compile",
            "compile_fragment",
            "CompileShader",
            "const",
            "continue",
            "ComputeShader",
            "ConsumeStructuredBuffer",
            "default",
            "DepthStencilState",
            "DepthStencilView",
            "discard",
            "do",
            "double",
            "DomainShader",
            "dword",
            "else",
            "export",
            "extern",
            "false",
            "float",
            "for",
            "foreach",
            "fxgroup",
            "GeometryShader",
            "groupshared",
            "half",
            "Hullshader",
            "if",
            "in",
            "inline",
            "inout",
            "InputPatch",
            "int",
            "interface",
            "line",
            "lineadj",
            "linear",
            "LineStream",
            "matrix",
            "min16float",
            "min10float",
            "min16int",
            "min12int",
            "min16uint",
            "namespace",
            "nointerpolation",
            "noperspective",
            "NULL",
            "out",
            "OutputPatch",
            "packoffset",
            "pass",
            "pixelfragment",
            "PixelShader",
            "point",
            "PointStream",
            "precise",
            "RasterizerState",
            "RenderTargetView",
            "return",
            "register",
            "rgroup",
            "row_major",
            "RWBuffer",
            "RWByteAddressBuffer",
            "RWStructuredBuffer",
            "RWTexture1D",
            "RWTexture1DArray",
            "RWTexture2D",
            "RWTexture2DArray",
            "RWTexture3D",
            "sample",
            "sampler",
            "SamplerState",
            "SamplerComparisonState",
            "shader",
            "shared",
            "snorm",
            "stateblock",
            "stateblock_state",
            "static",
            "stream",
            "string",
            "struct",
            "switch",
            "StructuredBuffer",
            "tbuffer",
            "technique",
            "technique10",
            "technique11",
            "texture",
            "Texture1D",
            "Texture1DArray",
            "Texture2D",
            "Texture2DArray",
            "Texture2DMS",
            "Texture2DMSArray",
            "Texture3D",
            "TextureCube",
            "TextureCubeArray",
            "true",
            "typedef",
            "triangle",
            "triangleadj",
            "TriangleStream",
            "uint",
            "uniform",
            "unorm",
            "unsigned",
            "var",
            "vector",
            "vertexfragment",
            "VertexShader",
            "void",
            "volatile",
            "while"
        ];
    }
}