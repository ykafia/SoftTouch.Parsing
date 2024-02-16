using System.Runtime.CompilerServices;
using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct ShaderClassParsers : IParser<ShaderClass>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out ShaderClass parsed, in ParseError? orError = null)
    {
        if(SimpleClass(ref scanner, result, out parsed, in orError))
            return true;
        else 
            return false;
    }
    public static bool Class(ref Scanner scanner, ParseResult result, out ShaderClass parsed, in ParseError? orError = null)
        => new ShaderClassParsers().Match(ref scanner, result, out parsed, in orError);
    public static bool SimpleClass(ref Scanner scanner, ParseResult result, out ShaderClass parsed, in ParseError? orError = null)
        => new SimpleShaderClassParser().Match(ref scanner, result, out parsed, in orError);
}

public record struct SimpleShaderClassParser : IParser<ShaderClass>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out ShaderClass parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;

        if(
            Terminals.Literal("shader", ref scanner, advance: true)
            && CommonParsers.Spaces1(ref scanner, result, out _, new("Expected at least one space", new(scanner, scanner.Position)))
            && LiteralsParser.Identifier(ref scanner, result, out var className, new("Expected class name", new(scanner, scanner.Position)))
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char('{', ref scanner, advance: true)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            
        )
        {
            var c = new ShaderClass(className, scanner.GetLocation(position, scanner.Position - position));
            while(!Terminals.Char('}', ref scanner, advance: true))
            {
                if(ShaderElementParsers.ShaderElement(ref scanner, result, out var e))
                {
                    c.Elements.Add(e);
                }
                CommonParsers.Spaces0(ref scanner, result, out _);
            }
            parsed = c;
            return true;
        }
        else
        {
            parsed = null!;
            scanner.Position = position;
            return false;
        }
    }
}