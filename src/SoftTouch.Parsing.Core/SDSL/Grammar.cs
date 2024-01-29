using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public static class Grammar
{
    public static ParseResult Match<T>(string code)
        where T : Node
    {
        var p = new NumberParser();
        var scanner = new Scanner(code);
        var result = new ParseResult();
        if (p.Match<NumberLiteral>(ref scanner, result, out var fnum))
            result.AST = fnum;
        Terminals.EOF(ref scanner);
        return result;
    }
}