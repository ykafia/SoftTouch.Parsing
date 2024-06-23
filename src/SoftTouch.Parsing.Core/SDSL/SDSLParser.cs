using SoftTouch.Parsing.SDSL.AST;
using SoftTouch.Parsing.SDSL.PreProcessing;

namespace SoftTouch.Parsing.SDSL;


public static class SDSLParser
{
    public static ParseResult Parse(string code)
    {
        var c = new CommentProcessedCode(code);
        return Grammar.Match<CommentProcessedCode, ShaderFileParser, ShaderFile>(c);
    }
}