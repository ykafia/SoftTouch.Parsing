using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public static class Grammar
{
    public static ParseResult Match<TParser, TValue>(string code, TParser? parser = null)
        where TValue : Node
        where TParser : struct, IParser<TValue>
    {
        var p = parser ?? new TParser();
        var scanner = new Scanner(code);
        var result = new ParseResult();
        if (p.Match(ref scanner, result, out var fnum))
            result.AST = fnum;
        Terminals.EOF(ref scanner);
        return result;
    }
}