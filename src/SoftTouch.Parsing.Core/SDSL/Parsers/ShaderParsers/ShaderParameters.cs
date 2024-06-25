using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct ParameterParsers : IParser<ParameterListNode>
{
    public bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ParameterListNode parsed, in ParseError? orError = null) where TScanner : struct, IScanner
    {
        throw new NotImplementedException();
    }
    public static bool Declarations<TScanner>(ref TScanner scanner, ParseResult result, out ShaderParameterDeclarations parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
        => new ParameterDeclarationsParser().Match(ref scanner, result, out parsed, in orError);
    public static bool Values<TScanner>(ref TScanner scanner, ParseResult result, out ShaderExpressionList parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
        => new ParameterListParser().Match(ref scanner, result, out parsed, in orError);
}


public record struct ParameterDeclarationsParser : IParser<ShaderParameterDeclarations>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ShaderParameterDeclarations parsed, in ParseError? orError = null) where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        List<ShaderParameter> parameters = [];
        while (
            LiteralsParser.TypeName(ref scanner, result, out var typename)
            && CommonParsers.Spaces1(ref scanner, result, out _)
            && LiteralsParser.Identifier(ref scanner, result, out var name)
            && CommonParsers.Spaces0(ref scanner, result, out _)
        )
        {
            parameters.Add(new(typename, name));
            if (!Terminals.Char(',', ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
                break;
        }
        parsed = new(scanner.GetLocation(position..scanner.Position)) { Parameters = parameters };
        return true;
    }
}
public record struct ParameterListParser : IParser<ShaderExpressionList>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ShaderExpressionList parsed, in ParseError? orError = null) where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        List<Expression> values = [];
        while (ExpressionParser.Expression(ref scanner, result, out var expr) && CommonParsers.Spaces0(ref scanner, result, out _))
        {
            values.Add(expr);
            if (!Terminals.Char(',', ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
                break;
        }
        parsed = new(scanner.GetLocation(position..scanner.Position))
        {
            Values = values
        };
        return true;
    }
}

public record struct GenericsListParser : IParser<ShaderExpressionList>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ShaderExpressionList parsed, in ParseError? orError = null) where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        List<Expression> values = [];
        while (ExpressionParser.Expression(ref scanner, result, out var expr) && CommonParsers.Spaces0(ref scanner, result, out _))
        {
            values.Add(expr);
            if (!Terminals.Char(',', ref scanner, advance: true) && CommonParsers.Spaces0(ref scanner, result, out _))
                break;
        }
        parsed = new(scanner.GetLocation(position..scanner.Position))
        {
            Values = values
        };
        return true;
    }
}