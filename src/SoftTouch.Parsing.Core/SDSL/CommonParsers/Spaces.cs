using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct Space0(bool OnlyWhiteSpace) : IParser<NoNode>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out NoNode parsed, in ParseError? error = null!)
    {
        parsed = null!;
        if (!OnlyWhiteSpace)
        {
            while (char.IsWhiteSpace((char)scanner.Peek()))
                scanner.Advance(1);
            return true;
        }
        else
        {
            while (scanner.Peek() == ' ')
                scanner.Advance(1);
            return true;
        }
    }
}

public record struct Space1(bool OnlyWhiteSpace) : IParser<NoNode>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out NoNode parsed, in ParseError? orError = null)
    {
        parsed = null!;
        if (!OnlyWhiteSpace)
        {
            if (!char.IsWhiteSpace((char)scanner.Peek()))
            {
                if (orError != null)
                    result.Errors.Add(orError.Value);
                return false;

            }
            while (char.IsWhiteSpace((char)scanner.Peek()))
                scanner.Advance(1);
            return true;
        }
        else
        {
            if (!(scanner.Peek() == ' '))
            {
                if (orError != null)
                    result.Errors.Add(orError.Value);
                return false;

            }
            while (scanner.Peek() == ' ')
                scanner.Advance(1);
            return true;
        }
    }
}