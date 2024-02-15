using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct StatementParsers : IParser<Statement>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
    {
        if (Expression(ref scanner, result, out parsed))
            return true;
        else if (Break(ref scanner, result, out parsed))
            return true;
        else if (Continue(ref scanner, result, out parsed))
            return true;
        else if (Declare(ref scanner, result, out parsed))
            return true;
        else if (DeclareAssign(ref scanner, result, out parsed))
            return true;
        else if (Block(ref scanner, result, out parsed))
            return true;
        if (orError is not null)
                result.Errors.Add(orError.Value);
        return false;
    }
    internal static bool Statement(ref Scanner scanner, ParseResult result, out Statement parsed)
        => new StatementParsers().Match(ref scanner, result, out parsed);
    internal static bool Block(ref Scanner scanner, ParseResult result, out Statement parsed)
        => new BlockStatementParser().Match(ref scanner, result, out parsed);
    internal static bool Break(ref Scanner scanner, ParseResult result, out Statement parsed)
        => new BreakParser().Match(ref scanner, result, out parsed);
    internal static bool Continue(ref Scanner scanner, ParseResult result, out Statement parsed)
    => new ContinueParser().Match(ref scanner, result, out parsed);
    internal static bool Expression(ref Scanner scanner, ParseResult result, out Statement parsed)
        => new ExpressionStatementParser().Match(ref scanner, result, out parsed);
    internal static bool Declare(ref Scanner scanner, ParseResult result, out Statement parsed)
        => new DeclareStatementParser().Match(ref scanner, result, out parsed);
    internal static bool DeclareAssign(ref Scanner scanner, ParseResult result, out Statement parsed)
        => new DeclareAssignStatementParser().Match(ref scanner, result, out parsed);
}


public record struct ReturnStatementParser : IParser<Statement>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        if (
            Terminals.Literal("return", ref scanner, advance: true)
            && CommonParsers.Spaces1(ref scanner, result, out _)
        )
        {
            if (Terminals.Char(';', ref scanner, advance: true))
            {
                parsed = new Return(scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else if (
                ExpressionParser.Expression(ref scanner, result, out var val)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && Terminals.Char(';', ref scanner, advance: true)
            )
            {
                parsed = new Return(scanner.GetLocation(position, scanner.Position - position), val);
                return true;
            }
            else
            {
                result.Errors.Add(new("Expected value or \";\"", new(scanner, scanner.Position)));
                parsed = null!;
                return false;
            }

        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}

public record struct BreakParser : IParser<Statement>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        if (
            Terminals.Literal("break", ref scanner, advance: true)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char(';', ref scanner, advance: true)
        )
        {
            parsed = new Break(scanner.GetLocation(position, scanner.Position - position));
            return true;
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}
public record struct ContinueParser : IParser<Statement>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        if (
            Terminals.Literal("continue", ref scanner, advance: true)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char(';', ref scanner, advance: true)
        )
        {
            parsed = new Break(scanner.GetLocation(position, scanner.Position - position));
            return true;
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}

public record struct ExpressionStatementParser : IParser<Statement>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        if (
            ExpressionParser.Expression(ref scanner, result, out var expression)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char(';', ref scanner, advance: true)
        )
        {
            parsed = new ExpressionStatement(expression, scanner.GetLocation(position, scanner.Position - position));
            return true;
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}


public record struct DeclareStatementParser : IParser<Statement>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        if (
            LiteralsParser.Identifier(ref scanner, result, out var typeName)
            && CommonParsers.Spaces1(ref scanner, result, out _)
            && LiteralsParser.Identifier(ref scanner, result, out var variableName)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char(';', ref scanner, advance: true)
        )
        {
            parsed = new Declare(typeName, variableName, scanner.GetLocation(position, scanner.Position - position));
            return true;
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}

public record struct DeclareAssignStatementParser : IParser<Statement>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        if (
            LiteralsParser.Identifier(ref scanner, result, out var typeName)
            && CommonParsers.Spaces1(ref scanner, result, out _)
            && LiteralsParser.Identifier(ref scanner, result, out var variableName)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char('=', ref scanner, advance: true)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && ExpressionParser.Expression(ref scanner, result, out var value)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char(';', ref scanner, advance: true)
        )
        {
            parsed = new DeclareAssign(typeName, variableName, value, scanner.GetLocation(position, scanner.Position - position));
            return true;
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}


public record struct BlockStatementParser : IParser<Statement>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        if (Terminals.Char('{', ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
        {
            var block = new BlockStatement(new());

            while (!Terminals.Char('}', ref scanner, advance: true))
            {
                if (StatementParsers.Statement(ref scanner, result, out var statement))
                {
                    block.Statements.Add(statement);
                    CommonParsers.Spaces0(ref scanner, result, out _);
                }
                else
                {
                    result.Errors.Add(new("Expected Statement", new ErrorLocation(scanner, scanner.Position)));
                    parsed = null!;
                    return false;
                }
            }
            block.Info = scanner.GetLocation(position, scanner.Position - position);
            parsed = block;
            return true;
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}


