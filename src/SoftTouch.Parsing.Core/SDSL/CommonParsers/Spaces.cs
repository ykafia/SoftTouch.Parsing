using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public struct Space0 : IParser<NoNode>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out NoNode parsed, in ParseError? error = null!)
    {
        parsed = null!;
        while(char.IsWhiteSpace((char)scanner.Peek()))
            scanner.Advance(1);
        return true;
    }
}

public struct Space1 : IParser<NoNode>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out NoNode parsed, in ParseError? orError = null)
    {
        parsed = null!;
        if(!char.IsWhiteSpace((char)scanner.Peek()))
        {
            if(orError != null)
                result.Errors.Add(orError.Value);
            return false;

        }
        while (char.IsWhiteSpace((char)scanner.Peek()))
            scanner.Advance(1);
        return true;
    }
}