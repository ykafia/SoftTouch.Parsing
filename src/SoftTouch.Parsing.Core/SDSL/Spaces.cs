using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public struct Space0 : IParser<NoNode>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out NoNode parsed)
    {
        parsed = null!;
        while(char.IsWhiteSpace((char)scanner.Peek()))
            scanner.Advance(1);
        return true;
    }
}

public struct Space1 : IParser<NoNode>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out NoNode parsed)
    {
        parsed = null!;
        if(!char.IsWhiteSpace((char)scanner.Peek()))
            return false;
        while (char.IsWhiteSpace((char)scanner.Peek()))
            scanner.Advance(1);
        return true;
    }
}