using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public record struct PostfixParser : IParser<Expression>
{

    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed)
    {
        var position = scanner.Position;
        // If the following 
        if (
            Accessor(ref scanner, result, out parsed)
            && CommonParsers.Spaces0(ref scanner, result, out _)
        )
        {
            if (Terminals.Set("[.", ref scanner) || Terminals.Literal("++", ref scanner) || Terminals.Literal("--", ref scanner))
            {
                if (Terminals.Char('.', ref scanner, advance: true))
                {
                    if (Postfix(ref scanner, result, out var accessed))
                    {
                        parsed = new AccessorExpression(parsed, accessed, scanner.GetLocation(position, scanner.Position));
                        return true;
                    }
                    else
                    {
                        scanner.Position = position;
                        return false;
                    }
                }
                else if (Terminals.Char('[', ref scanner, advance: true))
                {
                    CommonParsers.Spaces0(ref scanner, result, out _);
                    if (
                        ExpressionParser.Expression(ref scanner, result, out var index)
                        && CommonParsers.Spaces0(ref scanner, result, out _)
                        && Terminals.Char(']', ref scanner, advance: true)
                    )
                    {
                        parsed = new IndexerExpression(parsed, index, scanner.GetLocation(position, scanner.Position - position));
                        return true;
                    }
                    else
                    {
                        scanner.Position = position;
                        return false;
                    }
                }
                else if (Terminals.Literal("++", ref scanner, advance: true))
                {
                    parsed = new PostfixExpression(parsed, Operator.Inc, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else if (Terminals.Literal("--", ref scanner, advance: true))
                {
                    parsed = new PostfixExpression(parsed, Operator.Dec, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else 
                {
                    result.Errors.Add(new("Expected Postfix expression", scanner.GetLocation(scanner.Position, 1)));
                    return false;
                }
            }
            else return true;
        }
        else 
        {
            scanner.Position = position;
            return false;
        }
    }
    public static bool Postfix(ref Scanner scanner, ParseResult result, out Expression parsed)
        => new PostfixParser().Match(ref scanner, result, out parsed);
    internal static bool Increment(ref Scanner scanner, ParseResult result, out Expression parsed)
        => new PostfixIncrementParser().Match(ref scanner, result, out parsed);
    internal static bool Accessor(ref Scanner scanner, ParseResult result, out Expression parsed)
        => new PostfixAccessorParser().Match(ref scanner, result, out parsed);
    internal static bool Indexer(ref Scanner scanner, ParseResult result, out Expression parsed)
        => new PostfixIndexerParser().Match(ref scanner, result, out parsed);
}


public record struct PostfixAccessorParser : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed)
    {
        var position = scanner.Position;
        if (PostfixParser.Indexer(ref scanner, result, out var expression))
        {
            var pos2 = scanner.Position;
            CommonParsers.Spaces0(ref scanner, result, out _);
            if (
                Terminals.Char('.', ref scanner, advance: true)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && PostfixParser.Accessor(ref scanner, result, out var accessed))
            {
                parsed = new AccessorExpression(expression, accessed, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else
            {
                scanner.Position = pos2;
                parsed = expression;
                return true;
            }
        }
        parsed = null!;
        return false;
    }
}

public record struct PostfixIndexerParser : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed)
    {
        var position = scanner.Position;

        if (PrimaryParsers.Primary(ref scanner, result, out var expression))
        {
            var pos2 = scanner.Position;
            CommonParsers.Spaces0(ref scanner, result, out _);
            if (Terminals.Char('[', ref scanner, advance: true))
            {
                if (
                    CommonParsers.Spaces0(ref scanner, result, out _)
                    && ExpressionParser.Expression(ref scanner, result, out var index)
                    && CommonParsers.Spaces0(ref scanner, result, out _)
                    && Terminals.Char(']', ref scanner, advance: true)
                )
                {
                    parsed = new IndexerExpression(expression, index, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else 
                {
                    result.Errors.Add(new("Expected accessor parser",scanner.GetLocation(scanner.Position, 1)));
                    parsed = null!;
                    return false;
                }
            }
            else
            {
                scanner.Position = pos2;
                parsed = expression;
                return true;
            }
        }
        parsed = null!;
        return false;
    }
}

public record struct PostfixIncrementParser : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed)
    {
        var position = scanner.Position;
        if(PostfixParser.Accessor(ref scanner, result, out parsed))
        {
            var pos2 = scanner.Position;
            CommonParsers.Spaces0(ref scanner, result, out _);
            if(Terminals.Literal("++", ref scanner, advance: true))
            {
                parsed = new PostfixExpression(parsed, Operator.Inc, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else
            {
                scanner.Position = pos2;
                return true;
            }
        }
        parsed = null!;
        return false;
    }
}


