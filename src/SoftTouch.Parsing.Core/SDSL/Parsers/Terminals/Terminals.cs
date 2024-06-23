
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SoftTouch.Parsing.SDSL;


public static class Terminals
{
    public static bool AnyChar<TScanner>(ref TScanner scanner)
        where TScanner : struct, IScanner
        => !scanner.IsEof;

    public static CharTerminalParser Char(char c) => new(c);
    public static bool Char<TScanner>(char c, ref TScanner scanner, bool advance = false)
        where TScanner : struct, IScanner
         => new CharTerminalParser(c).Match(ref scanner, advance);
    public static SetTerminalParser Set(string set) => new(set);
    public static bool Set<TScanner>(string set, ref TScanner scanner, bool advance = false)
        where TScanner : struct, IScanner
        => new SetTerminalParser(set).Match(ref scanner, advance);
    public static LiteralTerminalParser Literal(string literal) => new(literal);
    public static bool Literal<TScanner>(string c, ref TScanner scanner, bool advance = false)
        where TScanner : struct, IScanner
        => new LiteralTerminalParser(c).Match(ref scanner, advance);
    public static DigitTerminalParser Digit(DigitRange? mode = null) => new(mode ?? DigitRange.All);
    public static bool Digit<TScanner>(ref TScanner scanner, DigitRange? mode = null, bool advance = false)
        where TScanner : struct, IScanner
        => new DigitTerminalParser(mode ?? DigitRange.All).Match(ref scanner, advance);
    public static LetterTerminalParser Letter() => new();
    public static bool Letter<TScanner>(ref TScanner scanner, bool advance = false)
        where TScanner : struct, IScanner
        => new LetterTerminalParser().Match(ref scanner, advance);
    public static LetterOrDigitTerminalParser LetterOrDigit() => new();
    public static bool LetterOrDigit<TScanner>(ref TScanner scanner, bool advance = false)
        where TScanner : struct, IScanner
        => new LetterOrDigitTerminalParser().Match(ref scanner, advance);
    public static bool IdentifierFirstChar<TScanner>(ref TScanner scanner, bool advance = false)
        where TScanner : struct, IScanner
        => Letter(ref scanner, advance) || Char('_', ref scanner, advance);
    public static bool EOL<TScanner>(ref TScanner scanner, bool advance = false)
        where TScanner : struct, IScanner
        => new EOLTerminalParser().Match(ref scanner, advance);
    public static bool EOF<TScanner>(ref TScanner scanner)
        where TScanner : struct, IScanner
        => new EOFTerminalParser().Match(ref scanner, false);
}

public interface ITerminal
{
    public bool Match<TScanner>(ref TScanner scanner, bool advance)
        where TScanner : struct, IScanner;
}

public record struct CharTerminalParser(char Character) : ITerminal
{
    public readonly bool Match<TScanner>(ref TScanner scanner, bool advance)
        where TScanner : struct, IScanner
    {
        if (advance && scanner.Peek() == Character)
        {
            scanner.Advance(1);
            return true;
        }
        return false;
    }
    public static implicit operator CharTerminalParser(char c) => new(c);
}

public struct DigitRange
{
    public static DigitRange All { get; } = new("[0-9]");
    public static DigitRange ExceptZero { get; } = new("[1-9]");
    public static DigitRange OnlyZero { get; } = new("0");
    public string Chars { get; set; }
    public DigitRange(string numbers)
    {
        if (numbers.Length == 1 && char.IsDigit(numbers[0]))
            Chars = numbers;
        else if (numbers.StartsWith('[') && numbers[2] == '-' && numbers.Length == 5)
        {
            int start = numbers[1] - '0';
            int end = numbers[3] - '0';
            var size = end - start;
            Span<char> span = stackalloc char[size];
            for (int i = start; i <= end; i++)
                span[i - start] = (char)(i + '0');
            Chars = span.ToString();
        }
        else
        {
            foreach(var d in numbers)
                if(!char.IsDigit(d))
                    throw new ArgumentException($"Cannot parse '{numbers}', it should be formatted as '[<start>-<end>]' or just a list of all digit characters needed");
            Chars = numbers.ToString();
        }
    }

    public static implicit operator DigitRange(string numbers) => new(numbers);
}

public record struct DigitTerminalParser(DigitRange Mode) : ITerminal
{
    public readonly bool Match<TScanner>(ref TScanner scanner, bool advance)
        where TScanner : struct, IScanner
    {
        bool found = false;
        if (Mode.Chars.Contains((char)scanner.Peek()))
            found = true;
        if (advance && found)
            scanner.Advance(1);
        return found;
    }
}

public record struct LetterTerminalParser() : ITerminal
{
    public readonly bool Match<TScanner>(ref TScanner scanner, bool advance)
        where TScanner : struct, IScanner
    {
        if (scanner.Peek() > 0 && char.IsLetter((char)scanner.Peek()))
        {
            if (advance)
                scanner.Advance(1);
            return true;
        }
        return false;
    }
}
public record struct LetterOrDigitTerminalParser() : ITerminal
{
    public readonly bool Match<TScanner>(ref TScanner scanner, bool advance)
        where TScanner : struct, IScanner
    {
        if (scanner.Peek() > 0 && char.IsLetterOrDigit((char)scanner.Peek()))
        {
            if (advance)
                scanner.Advance(1);
            return true;
        }
        return false;
    }
}

public record struct LiteralTerminalParser(string Literal, bool CaseSensitive = true) : ITerminal
{
    public readonly bool Match<TScanner>(ref TScanner scanner, bool advance)
        where TScanner : struct, IScanner
    {
        if (scanner.ReadString(Literal, CaseSensitive))
        {
            if (advance)
                scanner.Advance(Literal.Length);
            return true;
        }
        return false;
    }
    public static implicit operator LiteralTerminalParser(string lit) => new(lit);
}


public record struct SetTerminalParser(string Set) : ITerminal
{
    public readonly bool Match<TScanner>(ref TScanner scanner, bool advance)
        where TScanner : struct, IScanner
    {
        if (scanner.Peek() > 0 && Set.Contains((char)scanner.Peek()))
        {
            if (advance)
                scanner.Advance(1);
            return true;
        }
        return false;
    }

    public static implicit operator SetTerminalParser(string set) => new(set);
}

public record struct EOFTerminalParser() : ITerminal
{
    public readonly bool Match<TScanner>(ref TScanner scanner, bool advance)
        where TScanner : struct, IScanner
    {
        return scanner.IsEof;
    }
}
public record struct EOLTerminalParser() : ITerminal
{
    public readonly bool Match<TScanner>(ref TScanner scanner, bool advance)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        while (scanner.Peek() == ' ')
            scanner.Advance(1);
        var result = Terminals.Char('\n', ref scanner, advance) || Terminals.Literal("\r\n", ref scanner, advance);
        if (!advance && result)
            scanner.Position = position;
        return result;
    }
}

