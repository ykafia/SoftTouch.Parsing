using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct UnaryParsers : IParser<UnaryExpression>
{
    public bool Match(ref Scanner scanner, ParseResult result, out UnaryExpression parsed)
    {
        throw new NotImplementedException();
    }

    public static bool Prefix(ref Scanner scanner, ParseResult result, out PrefixExpression prefix)
        => new PrefixParser().Match(ref scanner, result, out prefix);
    public static bool Postfix(ref Scanner scanner, ParseResult result, out PostfixExpression postfix)
       => new PostfixParser().Match(ref scanner, result, out postfix);
}


public record struct PrefixParser : IParser<PrefixExpression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out PrefixExpression parsed)
    {
        var ws0 = new Space0();
        var position = scanner.Position;
        if(Terminals.Literal("++", ref scanner))
        {
            scanner.Advance(2);
            ws0.Match(ref scanner, result, out _);
            if(LiteralsParser.Literal(ref scanner, result, out var lit))
            {
                parsed = new(Operator.Inc, new ValueExpression(lit), scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else 
            {
                parsed = null!;
                scanner.Position = position;
                return false;
            }
        }
        else if(Terminals.Literal("--", ref scanner))
        {

        }
        throw new NotImplementedException();
    }
}

public record struct PostfixParser : IParser<PostfixExpression>
{
    public bool Match(ref Scanner scanner, ParseResult result, out PostfixExpression parsed)
    {
        throw new NotImplementedException();
    }
}