using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct PrimaryParsers : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed)
    {
        if(Parenthesis(ref scanner, result, out parsed))
            return true;
        else if(Method(ref scanner, result, out parsed))
            return true;
        else if(LiteralsParser.Literal(ref scanner, result, out var lit))
        {
            parsed = new ValueExpression(lit);
            return true;
        }
        else return false;
    }
    public static bool Primary(ref Scanner scanner, ParseResult result, out Expression parsed)
            => new PrimaryParsers().Match(ref scanner, result, out parsed);
    public static bool Identifier(ref Scanner scanner, ParseResult result, out Identifier parsed)
            => new IdentifierParser().Match(ref scanner, result, out parsed);
    public static bool Method(ref Scanner scanner, ParseResult result, out Expression parsed)
        => new MethodCallParser().Match(ref scanner, result, out parsed);
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

public record struct MethodCallParser : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed)
    {
        var position = scanner.Position;
        if (
            LiteralsParser.Identifier(ref scanner, result, out var identifier)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char('(', ref scanner, advance: true)
        )
        {
            CommonParsers.Spaces0(ref scanner, result, out _);
            var pos2 = scanner.Position;
            if (Terminals.Char(')', ref scanner, advance: true))
            {
                parsed = new MethodCall(identifier, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else if(ExpressionParser.Expression(ref scanner, result, out var first))
            {
                var method = new MethodCall(identifier, scanner.GetLocation(position, scanner.Position - position));
                method.Parameters.Add(first);
                CommonParsers.Spaces0(ref scanner, result, out _);
                while(Terminals.Char(',', ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
                {
                    if(ExpressionParser.Expression(ref scanner, result, out var param))
                        method.Parameters.Add(param);
                    CommonParsers.Spaces0(ref scanner, result, out _);
                }
                if(Terminals.Char(')', ref scanner, advance: true))
                {
                    parsed = method;
                    return true;   
                }
                else
                {
                    result.Errors.Add(new("Expected parenthesis for closing method call",new(scanner,position)));
                    scanner.Position = scanner.Code.Length;
                    parsed = null!;
                    return false;
                }
            }
            else 
            {
                
                scanner.Position = position;
                result.Errors.Add(new("Expected method call", new(scanner, position)));
                scanner.Position = scanner.Code.Length;
                parsed = null!;
                return false;
            }
        }
        else
        {
            parsed = null!;
            scanner.Position = position;
            return false;
        }
    }
}