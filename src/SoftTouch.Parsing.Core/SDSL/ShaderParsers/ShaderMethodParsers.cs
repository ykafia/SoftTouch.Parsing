using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct ShaderMethodParsers : IParser<ShaderElement>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out ShaderElement parsed, in ParseError? orError = null)
    {
        if(Simple(ref scanner, result, out var method, in orError))
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

    public static bool Simple(ref Scanner scanner, ParseResult result, out ShaderMethod parsed, in ParseError? orError = null)
        => new SimpleMethodParser().Match(ref scanner, result, out parsed, in orError);
}

public record struct SimpleMethodParser : IParser<ShaderMethod>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out ShaderMethod parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        if (
            LiteralsParser.Identifier(ref scanner, result, out var typename)
            && CommonParsers.Spaces1(ref scanner, result, out _, new("Expected at least one space", new(scanner, scanner.Position)))
            && LiteralsParser.Identifier(ref scanner, result, out var methodName)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char('(', ref scanner, advance: true)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char(')', ref scanner, advance: true)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && StatementParsers.Block(ref scanner, result, out var body)
        )
        {
            parsed = new ShaderMethod(methodName, typename, scanner.GetLocation(position, scanner.Position - position)) { Body = (BlockStatement)body };
            return true;
        }
        else
        {
            if (orError != null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}