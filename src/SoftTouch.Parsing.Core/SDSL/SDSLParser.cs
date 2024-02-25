using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public static class SDSLParser
{
    public static ParseResult Parse(string code)
    {
        var c = code;
        if (code.Contains("/*") || code.Contains("//"))
            c = Grammar.MatchTyped<CodeNodeParsers, CodeSnippets>(c).AST?.ToCode() ?? throw new NotImplementedException();
        return Grammar.Match<ShaderFileParser, ShaderFile>(c);
    }
}