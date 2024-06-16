using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct PrimaryParsers : IParser<Expression>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        if (Parenthesis(ref scanner, result, out parsed))
            return true;
        else if (Method(ref scanner, result, out parsed))
            return true;
        else if (LiteralsParser.Literal(ref scanner, result, out var lit))
        {
            parsed = lit;
            return true;
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            return false;
        }
    }
    public static bool Primary<TScanner>(ref TScanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
            => new PrimaryParsers().Match(ref scanner, result, out parsed, in orError);
    public static bool Identifier<TScanner>(ref TScanner scanner, ParseResult result, out Identifier parsed)
        where TScanner : struct, IScanner
            => new IdentifierParser().Match(ref scanner, result, out parsed);
    public static bool Method<TScanner>(ref TScanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
        => new MethodCallParser().Match(ref scanner, result, out parsed, in orError);
    public static bool Parenthesis<TScanner>(ref TScanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
        => new ParenthesisExpressionParser().Match(ref scanner, result, out parsed, in orError);
}


public record struct ParenthesisExpressionParser : IParser<Expression>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if (
            Terminals.Char('(', ref scanner, advance: true)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && ExpressionParser.Expression(ref scanner, result, out parsed, new("Expected expression value", scanner.CreateError(position)))
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char(')', ref scanner, advance: true)
        )
            return true;
        else
        {
            if (orError != null)
                result.Errors.Add(orError.Value);
            parsed = null!;
            scanner.Position = position;
            return false;
        }
    }
}

public record struct MethodCallParser : IParser<Expression>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
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
            else if (ExpressionParser.Expression(ref scanner, result, out var first))
            {
                var method = new MethodCall(identifier, scanner.GetLocation(position, scanner.Position - position));
                method.Parameters.Add(first);
                CommonParsers.Spaces0(ref scanner, result, out _);
                while (!scanner.IsEof && Terminals.Char(',', ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
                {
                    if (ExpressionParser.Expression(ref scanner, result, out var param))
                        method.Parameters.Add(param);
                    else
                    {
                        result.Errors.Add(new("Expected expression value", scanner.CreateError(scanner.Position)));
                        scanner.Position = scanner.Span.Length;
                        parsed = null!;
                        return false;
                    }
                    CommonParsers.Spaces0(ref scanner, result, out _);
                }
                if (Terminals.Char(')', ref scanner, advance: true))
                {
                    parsed = method;
                    return true;
                }
                else
                {
                    result.Errors.Add(new("Expected parenthesis for closing method call", scanner.CreateError(position)));
                    scanner.Position = scanner.Span.Length;
                    parsed = null!;
                    return false;
                }
            }
            else
            {

                scanner.Position = position;
                result.Errors.Add(new("Expected method call", scanner.CreateError(position)));
                scanner.Position = scanner.Span.Length;
                parsed = null!;
                return false;
            }
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            parsed = null!;
            scanner.Position = position;
            return false;
        }
    }
}