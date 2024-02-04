using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public record struct PostfixParser : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed)
    {
        if(Increment(ref scanner, result, out var nparsed))
        {
            parsed = nparsed;
            return true;
        }
        else if (Accessor(ref scanner, result, out var aparsed))
        {
            parsed = aparsed;
            return true;
        }
        else if (Indexer(ref scanner, result, out var idparsed))
        {
            parsed = idparsed;
            return true;
        }
        else if(PrimaryParsers.Primary(ref scanner, result, out parsed))
            return true;
        else 
        {
            result.Errors.Add(new("Expected Postfix parser here", scanner.GetLocation(scanner.Position, 1)));
            parsed = null!;
            return false;
        }
    }

    public static bool Increment(ref Scanner scanner, ParseResult result, out PostfixExpression parsed)
        => new PostfixIncrementParser().Match(ref scanner, result, out parsed);
    public static bool Accessor(ref Scanner scanner, ParseResult result, out PostfixExpression parsed)
        => new PostfixAccessorParser().Match(ref scanner, result, out parsed);
    public static bool Indexer(ref Scanner scanner, ParseResult result, out PostfixExpression parsed)
        => new PostfixIndexerParser().Match(ref scanner, result, out parsed);
}

public record struct PostfixIncrementParser : IParser<PostfixExpression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out PostfixExpression parsed)
    {
        var position = scanner.Position;
        parsed = null!;
        if (PostfixParser.Indexer(ref scanner, result, out var indexer))
        {
            if (
                CommonParsers.Spaces0(ref scanner, result, out _)
                && (Terminals.Literal("++", ref scanner) || Terminals.Literal("--", ref scanner))
            )
            {
                var op = scanner.Span.Slice(scanner.Position, 2).ToOperator();
                scanner.Advance(2);
                parsed = new PostfixExpression(indexer, op, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else
            {
                scanner.Position = position;
                return false;
            }
        }
        else if (PostfixParser.Accessor(ref scanner, result, out var accessor))
        {
            if (
                CommonParsers.Spaces0(ref scanner, result, out _)
                && (Terminals.Literal("++", ref scanner) || Terminals.Literal("--", ref scanner))
            )
            {
                var op = scanner.Span.Slice(scanner.Position, 2).ToOperator();
                scanner.Advance(2);
                parsed = new PostfixExpression(accessor, op, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else
            {
                scanner.Position = position;
                return false;
            }
        }
        if (LiteralsParser.Identifier(ref scanner, result, out var identifier))
        {
            if (
                CommonParsers.Spaces0(ref scanner, result, out _)
                && (Terminals.Literal("++", ref scanner) || Terminals.Literal("--", ref scanner))
            )
            {
                var op = scanner.Span.Slice(scanner.Position, 2).ToOperator();
                scanner.Advance(2);
                parsed = new PostfixExpression(new ValueExpression(identifier), op, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else
            {
                scanner.Position = position;
                return false;
            }
        }
        else
        {
            scanner.Position = position;
            return false;
        }
    }
}
public record struct PostfixAccessorParser : IParser<PostfixExpression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out PostfixExpression parsed)
    {
        var position = scanner.Position;
        parsed = null!;
        if(PostfixParser.Accessor(ref scanner, result, out var accessor))
        {
            if(
                CommonParsers.Spaces0(ref scanner, result, out _)
                && Terminals.Char('.', ref scanner)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && LiteralsParser.Identifier(ref scanner, result, out var identifier)
            )
            {
                parsed = new AccessorExpression(accessor, identifier, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else 
            {
                scanner.Position = position;
                return false;
            }
        }
        else if(PostfixParser.Indexer(ref scanner, result, out var indexer))
        {
            if (
                CommonParsers.Spaces0(ref scanner, result, out _)
                && Terminals.Char('.', ref scanner)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && LiteralsParser.Identifier(ref scanner, result, out var identifier)
            )
            {
                parsed = new AccessorExpression(indexer, identifier, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else 
            {
                scanner.Position = position;
                return false;
            }
        }
        else if (PostfixParser.Increment(ref scanner, result, out var increment))
        {
            if (
                CommonParsers.Spaces0(ref scanner, result, out _)
                && Terminals.Char('.', ref scanner)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && LiteralsParser.Identifier(ref scanner, result, out var identifier)
            )
            {
                parsed = new AccessorExpression(increment, identifier, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else
            {
                scanner.Position = position;
                return false;
            }
        }
        else {
            scanner.Position = position;
            return false;
        }
    }
}

public record struct PostfixIndexerParser : IParser<PostfixExpression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out PostfixExpression parsed)
    {
        var position = scanner.Position;
        parsed = null!;
        if (PostfixParser.Accessor(ref scanner, result, out var accesor))
        {
            if (
                CommonParsers.Spaces0(ref scanner, result, out _)
                && Terminals.Char('[', ref scanner)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && ExpressionParser.Expression(ref scanner, result, out var expression)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && Terminals.Char(']', ref scanner)
            )
            {
                parsed = new IndexerExpression(accesor, expression, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else
            {
                scanner.Position = position;
                return false;
            }
        }
        else if (PostfixParser.Indexer(ref scanner, result, out var indexer))
        {
            if (
                CommonParsers.Spaces0(ref scanner, result, out _)
                && Terminals.Char('[', ref scanner)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && ExpressionParser.Expression(ref scanner, result, out var expression)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && Terminals.Char(']', ref scanner)
            )
            {
                parsed = new IndexerExpression(indexer, expression, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else
            {
                scanner.Position = position;
                return false;
            }
        }
        else if (PostfixParser.Increment(ref scanner, result, out var increment))
        {
            if (
                CommonParsers.Spaces0(ref scanner, result, out _)
                && Terminals.Char('[', ref scanner)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && ExpressionParser.Expression(ref scanner, result, out var expression)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && Terminals.Char(']', ref scanner)
            )
            {
                parsed = new IndexerExpression(increment, expression, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else
            {
                scanner.Position = position;
                return false;
            }
        }
        else
        {
            scanner.Position = position;
            return false;
        }
    }
}