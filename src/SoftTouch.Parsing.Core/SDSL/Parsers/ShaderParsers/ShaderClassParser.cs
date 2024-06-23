using System.Runtime.CompilerServices;
using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct ShaderClassParsers : IParser<ShaderClass>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ShaderClass parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        if(SimpleClass(ref scanner, result, out parsed, in orError))
            return true;
        else 
            return false;
    }
    public static bool Class<TScanner>(ref TScanner scanner, ParseResult result, out ShaderClass parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
        => new ShaderClassParsers().Match(ref scanner, result, out parsed, in orError);
    public static bool SimpleClass<TScanner>(ref TScanner scanner, ParseResult result, out ShaderClass parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
        => new SimpleShaderClassParser().Match(ref scanner, result, out parsed, in orError);
    public static bool GenericsDefinition<TScanner>(ref TScanner scanner, ParseResult result, out ShaderGenerics parsed)
        where TScanner : struct, IScanner
        => new ShaderGenericsDefinitionParser().Match(ref scanner, result, out parsed);
}

public record struct SimpleShaderClassParser : IParser<ShaderClass>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ShaderClass parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;

        if(
            Terminals.Literal("shader", ref scanner, advance: true)
            && CommonParsers.Spaces1(ref scanner, result, out _, new("Expected at least one space", scanner.CreateError(scanner.Position)))
            && LiteralsParser.Identifier(ref scanner, result, out var className, new("Expected class name", scanner.CreateError(scanner.Position)))
            && CommonParsers.Spaces0(ref scanner, result, out _)
            && Terminals.Char('{', ref scanner, advance: true)
            && CommonParsers.Spaces0(ref scanner, result, out _)
            
        )
        {
            var c = new ShaderClass(className, scanner.GetLocation(position, scanner.Position - position));
            while(!scanner.IsEof && !Terminals.Char('}', ref scanner, advance: true) )
            {
                if (ShaderElementParsers.ShaderElement(ref scanner, result, out var e))
                {
                    c.Elements.Add(e);
                }
                else
                    break;
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

public record struct ShaderClassParser : IParser<ShaderClass>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ShaderClass parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;

        if (
            Terminals.Literal("shader", ref scanner, advance: true)
            && CommonParsers.Spaces1(ref scanner, result, out _, new("Expected at least one space", scanner.CreateError(scanner.Position)))
            && LiteralsParser.Identifier(ref scanner, result, out var className, new("Expected class name", scanner.CreateError(scanner.Position)))
            && CommonParsers.Spaces0(ref scanner, result, out _)
        )
        {
            var c = new ShaderClass(className, scanner.GetLocation(position, scanner.Position - position));

            if(Terminals.Char('<', ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
            {
                while(!scanner.IsEof && !Terminals.Char('>', ref scanner, advance: true))
                {
                    if(ShaderClassParsers.GenericsDefinition(ref scanner, result, out var gen))
                        c.Generics.Add(gen);
                    else 
                    {
                        result.Errors.Add(new("Expected generics definition", scanner.CreateError(scanner.Position)));
                        scanner.Position = scanner.Span.Length;
                        parsed = null!;
                        return false;
                    }
                }
            }

            while (!scanner.IsEof && !Terminals.Char('}', ref scanner, advance: true))
            {
                if (ShaderElementParsers.ShaderElement(ref scanner, result, out var e))
                {
                    c.Elements.Add(e);
                }
                else 
                {
                    result.Errors.Add(new("Expected shader element", scanner.CreateError(scanner.Position)));
                    scanner.Position = scanner.Span.Length;
                    parsed = null!;
                    return false;
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



public record struct ShaderGenericsDefinitionParser : IParser<ShaderGenerics>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ShaderGenerics parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if(
            LiteralsParser.Identifier(ref scanner, result, out var typename)
            && CommonParsers.Spaces1(ref scanner, result, out _, new("Expected at least one space", scanner.CreateError(scanner.Position)))
            && LiteralsParser.Identifier(ref scanner, result, out var identifier)
        )
        {
            parsed = new ShaderGenerics(typename,identifier, scanner.GetLocation(position, scanner.Position - position));
            return true;
        }
        else 
        {
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}