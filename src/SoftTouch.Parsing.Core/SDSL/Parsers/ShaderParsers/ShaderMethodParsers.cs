using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct ShaderMethodParsers : IParser<ShaderMethod>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ShaderMethod parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        if (Method(ref scanner, result, out var method, in orError))
        {
            parsed = method;
            return true;
        }
        else
        {
            parsed = null!;
            return false;
        }
    }

    public static bool Method<TScanner>(ref TScanner scanner, ParseResult result, out ShaderMethod parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
        => new MethodParser().Match(ref scanner, result, out parsed, in orError);
    public static bool Simple<TScanner>(ref TScanner scanner, ParseResult result, out ShaderMethod parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
        => new SimpleMethodParser().Match(ref scanner, result, out parsed, in orError);

    public static bool Parameters<TScanner>(ref TScanner scanner, ParseResult result, out ShaderParameterDeclarations parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
        => new ParameterDeclarationsParser().Match(ref scanner, result, out parsed, orError);
}

public record struct SimpleMethodParser : IParser<ShaderMethod>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ShaderMethod parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if (
            LiteralsParser.TypeName(ref scanner, result, out var typename)
            && CommonParsers.Spaces1(ref scanner, result, out _, new("Expected at least one space", scanner.CreateError(scanner.Position)))
            && LiteralsParser.Identifier(ref scanner, result, out var methodName)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char('(', ref scanner, advance: true)
            && ShaderMethodParsers.Parameters(ref scanner, result, out var parameters)
            && Terminals.Char(')', ref scanner, advance: true)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && StatementParsers.Block(ref scanner, result, out var body, new("Expected Body declaration", scanner.CreateError(scanner.Position)))
        )
        {
            parsed = new ShaderMethod(typename, methodName, scanner.GetLocation(position, scanner.Position - position))
            {
                ParameterList = parameters,
                Body = (BlockStatement)body
            };
            return true;
        }
        else return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }
}


public record struct MethodParser : IParser<ShaderMethod>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ShaderMethod parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        parsed = null!;
        var position = scanner.Position;
        if (Terminals.Literal("abstract", ref scanner, advance: true) && CommonParsers.Spaces1(ref scanner, result, out _))
        {
            if (
                LiteralsParser.TypeName(ref scanner, result, out var typename, orError: new("Expected type name here", scanner.CreateError(scanner.Position)))
                && CommonParsers.Spaces1(ref scanner, result, out _)
                && LiteralsParser.Identifier(ref scanner, result, out var methodName, orError: new("Expected method name here", scanner.CreateError(scanner.Position)))
                && CommonParsers.Spaces0(ref scanner, result, out _)
            )
            {
                if (Terminals.Char('(', ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
                {
                    ShaderMethodParsers.Parameters(ref scanner, result, out var parameters);
                    CommonParsers.Spaces0(ref scanner, result, out _);
                    if (!Terminals.Char(')', ref scanner, advance: true))
                        return CommonParsers.Exit(ref scanner, result, out parsed, position, new("Expected closed parenthesis", scanner.CreateError(scanner.Position)));

                    CommonParsers.Spaces0(ref scanner, result, out _);
                    if (!Terminals.Char(';', ref scanner, advance: true))
                    {
                        if (orError != null)
                            return CommonParsers.Exit(ref scanner, result, out parsed, position, new("Expected semi colon", scanner.CreateError(scanner.Position)));
                        else return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
                    }
                    else
                    {
                        parsed = new(typename, methodName, scanner.GetLocation(position..scanner.Position), isAbstract: true)
                        {
                            ParameterList = parameters
                        };
                        return true;
                    }
                }
                else return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
            }
        }
        else
            scanner.Position = position;
        if (Terminals.AnyOf(["clone", "override"], ref scanner, out var matched, advance: true) && CommonParsers.Spaces1(ref scanner, result, out _))
        {
            var isClone = false;
            var isOverride = false;
            var tmpPos = scanner.Position;
            if (matched == "clone")
                isClone = true;
            else if (matched == "override")
                isOverride = true;

            CommonParsers.Spaces0(ref scanner, result, out _);
            if (ShaderMethodParsers.Simple(ref scanner, result, out parsed, orError))
            {
                parsed.IsClone = isClone;
                parsed.IsOverride = isOverride;
                parsed.Info = scanner.GetLocation(position..scanner.Position);
                return true;
            }
        }
        else
            scanner.Position = position;
        if (ShaderMethodParsers.Simple(ref scanner, result, out parsed, orError))
        {
            parsed.Info = scanner.GetLocation(position..scanner.Position);
            return true;
        }
        return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }

}

