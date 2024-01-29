using System.Collections.Frozen;
using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public interface ILiteralParser<TResult>
{
    public bool Match(ref Scanner scanner, ParseResult result, out TResult literal);
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