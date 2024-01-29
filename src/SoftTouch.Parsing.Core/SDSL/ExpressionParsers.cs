using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public struct AdditionParser : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed)
    {
        var position = scanner.Position;
        var ws0 = new Space0();
        parsed = null!;
        ws0.Match(ref scanner, result, out _);
        if (Literals.Literal(ref scanner, result, out var lit))
        {
            ws0.Match(ref scanner, result, out _);
            if(Terminals.Char('+', ref scanner))
            {

            }
            return true;
        }
        else
        {
            scanner.Position = position;
            return false;
        }
    }
}