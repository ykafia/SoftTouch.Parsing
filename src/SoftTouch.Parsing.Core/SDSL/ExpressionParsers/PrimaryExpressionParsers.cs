using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct PrimaryParsers : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed)
    {
        if(Parenthesis(ref scanner, result, out parsed))
            return true;
        else if(LiteralsParser.Literal(ref scanner, result, out var lit))
        {
            parsed = new ValueExpression(lit);
            return true;
        }
        else return false;
    }

    public static bool Parenthesis(ref Scanner scanner, ParseResult result, out Expression parsed)
        => new ParenthesisExpressionParser().Match(ref scanner, result, out parsed);
}


public record struct ParenthesisExpressionParser : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed)
    {
        var position = scanner.Position;
        if(
            Terminals.Char('(', ref scanner, advance: true)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && ExpressionParser.Expression(ref scanner, result, out parsed)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char(')', ref scanner, advance: true)
        )
            return true;
        else 
        {
            parsed = null!;
            scanner.Position = position;
            return false;
        }
    }
}