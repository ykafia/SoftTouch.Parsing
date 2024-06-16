
using System.Runtime.InteropServices;

namespace SoftTouch.Parsing.SDSL;


public static class Terminals
{
    public static bool AnyChar<TScanner>(ref TScanner scanner) 
        where TScanner : struct, IScanner
        => !scanner.IsEof;
    public static bool Char<TScanner>(char c, ref TScanner scanner, bool advance = false) 
        where TScanner : struct, IScanner
         => new CharTerminalParser(c).Match(ref scanner, advance);
    public static bool Set<TScanner>(string set, ref TScanner scanner, bool advance = false) 
        where TScanner : struct, IScanner
        => new SetTerminalParser(set).Match(ref scanner, advance);
    public static bool Literal<TScanner>(string c, ref TScanner scanner, bool advance = false) 
        where TScanner : struct, IScanner
        => new LiteralTerminalParser(c).Match(ref scanner, advance);
    public static bool Digit<TScanner>(ref TScanner scanner, DigitMode mode = DigitMode.All, bool advance = false) 
        where TScanner : struct, IScanner
        => new DigitTerminalParser(mode).Match(ref scanner, advance);
    public static bool Letter<TScanner>(ref TScanner scanner, bool advance = false) 
        where TScanner : struct, IScanner
        => new LetterTerminalParser().Match(ref scanner, advance);
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


public enum DigitMode
{
    All,
    ExceptZero,
    OnlyZero
}

public record struct DigitTerminalParser(DigitMode Mode) : ITerminal
{
    public readonly bool Match<TScanner>(ref TScanner scanner, bool advance)
        where TScanner : struct, IScanner
    {
        var found = (scanner.Peek(), Mode) switch
        {
            ( >= 0, DigitMode.All) => char.IsDigit((char)scanner.Peek()),
            ( >= 0, DigitMode.OnlyZero) => (char)scanner.Peek() == '0',
            ( >= 0, DigitMode.ExceptZero) => (char)scanner.Peek() != '0' && char.IsDigit((char)scanner.Peek()),
            _ => false
        };
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


