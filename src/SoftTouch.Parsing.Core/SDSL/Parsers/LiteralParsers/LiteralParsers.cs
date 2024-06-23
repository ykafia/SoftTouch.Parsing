using System.Collections.Frozen;
using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public interface ILiteralParser<TResult>
{
    public bool Match<TScanner>(ref TScanner scanner, ParseResult result, out TResult literal)
        where TScanner : struct, IScanner;
}

public record struct LiteralsParser : IParser<Literal>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Literal literal, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if(Identifier(ref scanner, result, out var i))
        {
            literal = i;
            return true;
        }
        else if (Number(ref scanner, result, out var n))
        {
            literal = n;
            return true;
        }
        else 
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            literal = null!;
            scanner.Position = position;
            return false;
        }
    }
    public static bool Literal<TScanner>(ref TScanner scanner, ParseResult result, out Literal literal, in ParseError? orError = null)
        where TScanner : struct, IScanner 
        => new LiteralsParser().Match(ref scanner, result, out literal, in orError);
    public static bool Identifier<TScanner>(ref TScanner scanner, ParseResult result, out Identifier identifier, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        if(new IdentifierParser().Match(ref scanner, result, out identifier))
            return true;
        else if(orError != null)
            result.Errors.Add(orError.Value);
        return false;
    }
    public static bool TypeName<TScanner>(ref TScanner scanner, ParseResult result, out TypeName typeName, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        if(new TypeNameParser().Match(ref scanner, result, out typeName))
            return true;
        else if(orError != null)
            result.Errors.Add(orError.Value);
        return false;
    }
    public static bool Number<TScanner>(ref TScanner scanner, ParseResult result, out NumberLiteral number, in ParseError? orError = null)
        where TScanner : struct, IScanner 
        => new NumberParser().Match(ref scanner, result, out number, in orError);
    public static bool Integer<TScanner>(ref TScanner scanner, ParseResult result, out IntegerLiteral number, in ParseError? orError = null)
        where TScanner : struct, IScanner 
        => new IntegerParser().Match(ref scanner, result, out number, in orError);
}


public readonly record struct OperatorParser() : ILiteralParser<Operator>
{
    static List<string> Operators { get; } = [
        "!",
        "~",
        "++",
        "--",
        "+",
        "-",
        "*",
        "/",
        "%",
        "<<",
        ">>",
        "&",
        "|",
        "^",
        "<",
        ">",
        "<=",
        ">=",
        "==",
        "!=",
        "&&",
        "||"
    ];
    public static bool TryMatchAndAdvance<TScanner>(ref TScanner scanner, string match, out Operator op)
        where TScanner : struct, IScanner
    {
        op = Operator.Nop;
        if (Terminals.Literal<TScanner>(match, ref scanner))
        {
            op = match.ToOperator();
            scanner.Advance(match.Length);
            return true;
        }
        return false;
    }

    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Operator op)
        where TScanner : struct, IScanner
    {
        op = Operator.Nop;
        foreach (var e in Operators)
            if (TryMatchAndAdvance(ref scanner, e, out op))
                return true;
        return false;
    }
}



public record struct Suffix(int Size, bool IsFloatingPoint, bool Signed)
{
    public readonly override string ToString()
    {
        return (IsFloatingPoint, Signed) switch 
        {
            (true, _) => $"f{Size}",
            (false, false) => $"u{Size}",
            (false, true) => $"i{Size}",
        };
    }
}

public readonly record struct FloatSuffixParser() : ILiteralParser<Suffix>
{
    public static bool TryMatchAndAdvance<TScanner>(ref TScanner scanner, string match)
        where TScanner : struct, IScanner
    {
        if (Terminals.Literal<TScanner>(match, ref scanner))
        {
            scanner.Advance(match.Length);
            return true;
        }
        return false;
    }

    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Suffix suffix)
        where TScanner : struct, IScanner
    {
        suffix = new(32, false, false);
        if (Terminals.Char<TScanner>('f', ref scanner))
        {
            scanner.Advance(1);
            if (TryMatchAndAdvance(ref scanner, "16"))
                suffix = new(16, true, true);
            else if (TryMatchAndAdvance(ref scanner, "32"))
                suffix = new(32, true, true);
            else if (TryMatchAndAdvance(ref scanner, "64"))
                suffix = new(64, true, true);
            else
                suffix = new(32, true, true);
            return true;
        }
        else if (Terminals.Char('d', ref scanner))
        {
            scanner.Advance(1);
            suffix = new(64, true, true);
            return true;
        }
        else if (Terminals.Char('h', ref scanner))
        {
            scanner.Advance(1);
            suffix = new(16, true, true);
            return true;
        }
        else return false;
    }
}

public readonly record struct IntegerSuffixParser() : ILiteralParser<Suffix>
{
    public static bool TryMatchAndAdvance<TScanner>(ref TScanner scanner, string match)
        where TScanner : struct, IScanner
    {
        if (Terminals.Literal(match, ref scanner))
        {
            scanner.Advance(match.Length);
            return true;
        }
        return false;
    }

    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Suffix suffix)
        where TScanner : struct, IScanner
    {
        suffix = new(32, false, false);
        if (Terminals.Char('i', ref scanner))
        {
            scanner.Advance(1);
            if (TryMatchAndAdvance(ref scanner, "8"))
                suffix = new(8, false, true);
            else if (TryMatchAndAdvance(ref scanner, "16"))
                suffix = new(16, false, true);
            else if (TryMatchAndAdvance(ref scanner, "32"))
                suffix = new(32, false, true);
            else if (TryMatchAndAdvance(ref scanner, "64"))
                suffix = new(64, false, true);
            return true;
        }
        else if (Terminals.Char('u', ref scanner))
        {
            scanner.Advance(1);
            if (TryMatchAndAdvance(ref scanner, "8"))
                suffix = new(8, false, false);
            else if (TryMatchAndAdvance(ref scanner, "16"))
                suffix = new(16, false, false);
            else if (TryMatchAndAdvance(ref scanner, "32"))
                suffix = new(32, false, false);
            else if (TryMatchAndAdvance(ref scanner, "64"))
                suffix = new(64, false, false);
            return true;
        }
        else if (Terminals.Char('l', ref scanner))
        {
            scanner.Advance(1);
            suffix = new(64, false, true);
            return true;
        }
        else return false;
    }
}


public record struct IdentifierParser() : ILiteralParser<Identifier>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Identifier literal)
        where TScanner : struct, IScanner
    {
        literal = null!;
        var position = scanner.Position;
        if(Terminals.Char('_', ref scanner) || Terminals.Letter(ref scanner))
        {
            scanner.Advance(1);
            while(Terminals.LetterOrDigit(ref scanner) || Terminals.Char('_', ref scanner))
                scanner.Advance(1);
            literal = new(scanner.Memory[position..scanner.Position].ToString(), scanner.GetLocation(position, scanner.Position - position));
            return true;
        }
        else return false;
    }
}

public record struct TypeNameParser() : ILiteralParser<TypeName>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out TypeName name)
        where TScanner : struct, IScanner
    {
        name = null!;
        var position = scanner.Position;
        if(LiteralsParser.Identifier(ref scanner, result, out var identifier))
        {
            var intermediate = scanner.Position;
            if(
                CommonParsers.Spaces0(ref scanner, result, out _) 
                && Terminals.Char('[', ref scanner, advance: true)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && CommonParsers.Optional(ref scanner, new ExpressionParser(), result, out _)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && Terminals.Char(']', ref scanner, advance: true)
            )
            {
                name = new TypeName(scanner.Memory[position..scanner.Position].ToString().Trim(), scanner.GetLocation(position..scanner.Position));
                return true;
            }
            else {
                scanner.Position = intermediate;
                name = new(identifier.Name, scanner.GetLocation(position..scanner.Position));
                return true;
            }
        }
        else return false;
    }
}