using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;



public record struct FlowParsers : IParser<Flow>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Flow parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if (While(ref scanner, result, out var w, orError))
        {
            parsed = w;
            return true;
        }
        else if (ForEach(ref scanner, result, out var fe, orError))
        {
            parsed = fe;
            return true;
        }
        // else if (For(ref scanner, result, out var f, orError))
        // {
        //     parsed = f;
        //     return true;
        // }
        else return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }

    public static bool While<TScanner>(ref TScanner scanner, ParseResult result, out While parsed, ParseError? orError = null)
        where TScanner : struct, IScanner
        => new WhileParser().Match(ref scanner, result, out parsed, orError);
    public static bool ForEach<TScanner>(ref TScanner scanner, ParseResult result, out ForEach parsed, ParseError? orError = null)
        where TScanner : struct, IScanner
        => new ForEachParser().Match(ref scanner, result, out parsed, orError);
    public static bool For<TScanner>(ref TScanner scanner, ParseResult result, out For parsed, ParseError? orError = null)
        where TScanner : struct, IScanner
        => new ForParser().Match(ref scanner, result, out parsed, orError);
}



public record struct ForParser : IParser<For>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out For parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if(
            Terminals.Literal("for", ref scanner, advance: true)
            && CommonParsers.FollowedBy(ref scanner, Terminals.Char('('), withSpaces: true)
        )
        {
            CommonParsers.Spaces0(ref scanner, result, out _);
            Terminals.Char('(', ref scanner, advance: true);
            throw new NotImplementedException();

        }
        else return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
        // if (
        //     Terminals.Literal("if", ref scanner, advance: true)
        //     && CommonParsers.Spaces0(ref scanner, result, out _)
        //     && Terminals.Char('(', ref scanner, advance: true)
        //     && CommonParsers.Spaces0(ref scanner, result, out _)
        //     && ExpressionParser.Expression(ref scanner, result, out var condition, new("Expected expression here", scanner.CreateError(scanner.Position)))
        //     && CommonParsers.Spaces0(ref scanner, result, out _)
        // )
        // {
        //     if (Terminals.Char(')', ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
        //     {
        //         if (StatementParsers.Statement(ref scanner, result, out var statement, new("Expected statement here", scanner.CreateError(scanner.Position))))
        //         {
        //             parsed = new(condition, statement, scanner.GetLocation(position..scanner.Position));
        //             return true;
        //         }
        //     }
        //     else return CommonParsers.Exit(ref scanner, result, out parsed, position, new("Expected closing parenthesis", scanner.CreateError(scanner.Position)));
        // }
        // return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }
}

public record struct ForEachParser : IParser<ForEach>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ForEach parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if (Terminals.Literal("foreach", ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
        {
            if (Terminals.Char('(', ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
            {
                if (
                    LiteralsParser.TypeName(ref scanner, result, out var typeName, new("Expected type definition here", scanner.CreateError(scanner.Position)))
                    && CommonParsers.Spaces1(ref scanner, result, out _)
                    && LiteralsParser.Identifier(ref scanner, result, out var identifier, new("Expected variable name here", scanner.CreateError(scanner.Position)))
                    && CommonParsers.Spaces1(ref scanner, result, out _)
                )
                {
                    if (Terminals.Literal("in", ref scanner, advance: true) && CommonParsers.Spaces1(ref scanner, result, out _))
                    {
                        if (
                            ExpressionParser.Expression(ref scanner, result, out var collection, new("Expected variable name or collection name here", scanner.CreateError(scanner.Position)))
                            && CommonParsers.Spaces0(ref scanner, result, out _)
                        )
                        {
                            if (Terminals.Char(')', ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
                            {
                                if (StatementParsers.Statement(ref scanner, result, out var statement, new("Expected statement to be here", scanner.CreateError(scanner.Position))))
                                {
                                    parsed = new(typeName, identifier, collection, statement, scanner.GetLocation(position..scanner.Position));
                                    return true;
                                }
                            }
                            else return CommonParsers.Exit(ref scanner, result, out parsed, position, new("Expected closing parenthesis here", scanner.CreateError(scanner.Position)));
                        }
                    }
                    else return CommonParsers.Exit(ref scanner, result, out parsed, position, new("Expected keyword in here", scanner.CreateError(scanner.Position)));
                }
            }
            else return CommonParsers.Exit(ref scanner, result, out parsed, position, new("Expected opening parenthesis here", scanner.CreateError(scanner.Position)));
        }
        return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }
}

public record struct WhileParser : IParser<While>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out While parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if (Terminals.Literal("while", ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
        {
            if (Terminals.Char('(', ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
            {
                if (ExpressionParser.Expression(ref scanner, result, out var expression, new("Expected expression here", scanner.CreateError(scanner.Position))))
                {
                    if (Terminals.Char(')', ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
                    {
                        if (StatementParsers.Statement(ref scanner, result, out var statement, new("Expected statement to be here", scanner.CreateError(scanner.Position))))
                        {
                            parsed = new(expression, statement, scanner.GetLocation(position..scanner.Position));
                            return true;
                        }
                    }
                    else return CommonParsers.Exit(ref scanner, result, out parsed, position, new("Expected closing parenthesis here", scanner.CreateError(scanner.Position)));
                }
            }
            else return CommonParsers.Exit(ref scanner, result, out parsed, position, new("Expected opening parenthesis here", scanner.CreateError(scanner.Position)));
        }
        return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }
}

