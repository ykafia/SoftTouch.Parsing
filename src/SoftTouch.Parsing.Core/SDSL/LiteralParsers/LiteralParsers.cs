using System.Collections.Frozen;
using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public interface ILiteralParser<TResult>
{
    public bool Match(ref Scanner scanner, ParseResult result, out TResult literal);
}

public record struct LiteralsParser : IParser<Literal>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Literal literal)
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
            literal = null!;
            scanner.Position = position;
            return false;
        }
    }
    public static bool Literal(ref Scanner scanner, ParseResult result, out Literal literal) => new LiteralsParser().Match(ref scanner, result, out literal);
    public static bool Identifier(ref Scanner scanner, ParseResult result, out Identifier identifier) => new IdentifierParser().Match(ref scanner, result, out identifier);
    public static bool Number(ref Scanner scanner, ParseResult result, out NumberLiteral number) => new NumberParser().Match(ref scanner, result, out number);
    public static bool Operator(ref Scanner scanner, ParseResult result, out Operator op) => new OperatorParser().Match(ref scanner, result, out op);
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
    public static bool TryMatchAndAdvance(ref Scanner scanner, string match, out Operator op)
    {
        op = Operator.Nop;
        if (Terminals.Literal(match, ref scanner))
        {
            op = match.ToOperator();
            scanner.Advance(match.Length);
            return true;
        }
        return false;
    }

    public readonly bool Match(ref Scanner scanner, ParseResult result, out Operator op)
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
    public static bool TryMatchAndAdvance(ref Scanner scanner, string match)
    {
        if (Terminals.Literal(match, ref scanner))
        {
            scanner.Advance(match.Length);
            return true;
        }
        return false;
    }

    public readonly bool Match(ref Scanner scanner, ParseResult result, out Suffix suffix)
    {
        suffix = new(32, false, false);
        if (Terminals.Char('f', ref scanner))
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
    public static bool TryMatchAndAdvance(ref Scanner scanner, string match)
    {
        if (Terminals.Literal(match, ref scanner))
        {
            scanner.Advance(match.Length);
            return true;
        }
        return false;
    }

    public readonly bool Match(ref Scanner scanner, ParseResult result, out Suffix suffix)
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
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Identifier literal)
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