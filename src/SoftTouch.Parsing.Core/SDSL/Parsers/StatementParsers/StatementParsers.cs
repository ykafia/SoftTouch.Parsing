using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct StatementParsers : IParser<Statement>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if (Controls(ref scanner, result, out var cond))
        {
            parsed = cond;
            return true;
        }
        else if (Flow(ref scanner, result, out var flow))
        {
            parsed = flow;
            return true;
        }
        else if (Expression(ref scanner, result, out parsed))
            return true;
        else if (Break(ref scanner, result, out parsed))
            return true;
        else if (Return(ref scanner, result, out parsed))
            return true;
        else if (Continue(ref scanner, result, out parsed))
            return true;
        else if (Declare(ref scanner, result, out parsed))
            return true;
        else if (DeclareAssign(ref scanner, result, out parsed))
            return true;
        else if (Block(ref scanner, result, out parsed))
            return true;
        return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }
    internal static bool Statement<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, ParseError? orError = null)
        where TScanner : struct, IScanner
        => new StatementParsers().Match(ref scanner, result, out parsed, orError);
    internal static bool Block<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, ParseError? orError = null)
        where TScanner : struct, IScanner
        => new BlockStatementParser().Match(ref scanner, result, out parsed, orError);
    internal static bool Break<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, ParseError? orError = null)
        where TScanner : struct, IScanner
        => new BreakParser().Match(ref scanner, result, out parsed, orError);
    internal static bool Return<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, ParseError? orError = null)
        where TScanner : struct, IScanner
        => new ReturnStatementParser().Match(ref scanner, result, out parsed, orError);
    internal static bool Continue<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, ParseError? orError = null)
        where TScanner : struct, IScanner
    => new ContinueParser().Match(ref scanner, result, out parsed, orError);
    internal static bool Expression<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, ParseError? orError = null)
        where TScanner : struct, IScanner
        => new ExpressionStatementParser().Match(ref scanner, result, out parsed, orError);
    internal static bool Declare<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, ParseError? orError = null)
        where TScanner : struct, IScanner
        => new DeclareStatementParser().Match(ref scanner, result, out parsed, orError);
    internal static bool DeclareAssign<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, ParseError? orError = null)
        where TScanner : struct, IScanner
        => new DeclareAssignStatementParser().Match(ref scanner, result, out parsed, orError);
    internal static bool Controls<TScanner>(ref TScanner scanner, ParseResult result, out ConditionalFlow parsed, ParseError? orError = null)
       where TScanner : struct, IScanner
       => new ControlsParser().Match(ref scanner, result, out parsed, orError);
    internal static bool Flow<TScanner>(ref TScanner scanner, ParseResult result, out Flow parsed, ParseError? orError = null)
      where TScanner : struct, IScanner
      => new FlowParsers().Match(ref scanner, result, out parsed, orError);
}


public record struct ReturnStatementParser : IParser<Statement>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if (Terminals.Literal("return;", ref scanner, advance: true))
        {
            parsed = new Return(scanner.GetLocation(position..scanner.Position));
            return true;
        }
        else if (
            Terminals.Literal("return", ref scanner, advance: true)
            && CommonParsers.Spaces1(ref scanner, result, out _, new("Expected at least one space", scanner.CreateError(scanner.Position)))
        )
        {
            if (Terminals.Char(';', ref scanner, advance: true))
            {
                parsed = new Return(scanner.GetLocation(position..scanner.Position));
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
            else return CommonParsers.Exit(ref scanner, result, out parsed, position, new("Expected value or \";\"", scanner.CreateError(scanner.Position)));


        }
        else return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }
}

public record struct BreakParser : IParser<Statement>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
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
        else return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }
}
public record struct ContinueParser : IParser<Statement>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
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
        else return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }
}

public record struct ExpressionStatementParser : IParser<Statement>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
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
        else return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }
}


public record struct DeclareStatementParser : IParser<Statement>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if (
            LiteralsParser.TypeName(ref scanner, result, out var typeName)
            && CommonParsers.Spaces1(ref scanner, result, out _, new("Expected at least one space", scanner.CreateError(scanner.Position)))
            && LiteralsParser.Identifier(ref scanner, result, out var variableName, new("Expected an identifier", scanner.CreateError(scanner.Position)))
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char(';', ref scanner, advance: true)
        )
        {
            parsed = new Declare(typeName, variableName, scanner.GetLocation(position, scanner.Position - position));
            return true;
        }
        else return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }
}

public record struct DeclarationStatementParser : IParser<Statement>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if (
            LiteralsParser.TypeName(ref scanner, result, out var typeName)
            && CommonParsers.Spaces1(ref scanner, result, out _, new("Expected at least one space", scanner.CreateError(scanner.Position)))
            && LiteralsParser.Identifier(ref scanner, result, out var variableName, new("Use of reserved name for identifier", scanner.CreateError(scanner.Position)))
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char('=', ref scanner, advance: true)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && ExpressionParser.Expression(ref scanner, result, out var value, new("Expected a value expression", scanner.CreateError(scanner.Position)))
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char(';', ref scanner, advance: true)
        )
        {
            parsed = new DeclareAssign(typeName, variableName, value, scanner.GetLocation(position, scanner.Position - position));
            return true;
        }
        else return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }
}

public record struct AssignmentsParser : IParser<Statement>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if (CommonParsers.Repeat(ref scanner, ))
        else return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }
}
public record struct NameValueParser : IParser<NameValue>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out NameValue parsed, in ParseError? orError = null) where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if (LiteralsParser.Identifier(ref scanner, result, out var identifier))
        {
            if (CommonParsers.FollowedBy(ref scanner, Terminals.Char('='), withSpaces: true, advance: true))
            {
                CommonParsers.Spaces0(ref scanner, result, out _);
                if(ExpressionParser.Expression(ref scanner, result, out var expression))
                {
                    parsed = new(identifier, scanner.GetLocation(position..scanner.Position), expression);
                    return true;
                }
                else return CommonParsers.Exit(ref scanner, result, out parsed, position, new("Expected expression here", scanner.CreateError(position)));
            }
            else
            {
                parsed = new(identifier, scanner.GetLocation(position..scanner.Position));
                return true;
            }
        }
        else return CommonParsers.Exit(ref scanner, result, out parsed, position);
    }
}



public record struct BlockStatementParser : IParser<Statement>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if (Terminals.Char('{', ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
        {
            var block = new BlockStatement(new());

            while (!scanner.IsEof && !Terminals.Char('}', ref scanner, advance: true))
            {
                if (StatementParsers.Statement(ref scanner, result, out var statement))
                {
                    block.Statements.Add(statement);
                    CommonParsers.Spaces0(ref scanner, result, out _);
                }
                else return CommonParsers.Exit(ref scanner, result, out parsed, position, new("Expected Statement", scanner.CreateError(scanner.Position)));
            }
            block.Info = scanner.GetLocation(position, scanner.Position - position);
            parsed = block;
            return true;
        }
        else return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }
}


