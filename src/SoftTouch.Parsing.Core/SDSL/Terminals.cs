
using System.Runtime.InteropServices;

namespace SoftTouch.Parsing.SDSL;


public static class Terminals
{
    public static bool AnyChar(ref Scanner scanner) => !scanner.IsEof;
    public static bool Char(char c, ref Scanner scanner, bool advance = false)  => new CharTerminalParser(c).Match(ref scanner, advance);
    public static bool Set(string set, ref Scanner scanner, bool advance = false) => new SetTerminalParser(set).Match(ref scanner, advance);
    public static bool Literal(string c, ref Scanner scanner, bool advance = false) => new LiteralTerminalParser(c).Match(ref scanner, advance);
    public static bool Digit(ref Scanner scanner, DigitMode mode = DigitMode.All, bool advance = false) => new DigitTerminalParser(mode).Match(ref scanner, advance);
    public static bool Letter(ref Scanner scanner, bool advance = false) => new LetterTerminalParser().Match(ref scanner, advance);
    public static bool LetterOrDigit(ref Scanner scanner, bool advance = false) => new LetterOrDigitTerminalParser().Match(ref scanner, advance);
    public static bool IdentifierFirstChar(ref Scanner scanner, bool advance = false) 
        => Letter(ref scanner,advance) || Char('_',ref scanner, advance);
    public static bool EOL(ref Scanner scanner, bool advance = false) => new EOLTerminalParser().Match(ref scanner, advance);
    public static bool EOF(ref Scanner scanner) => new EOFTerminalParser().Match(ref scanner, false);
}

public interface ITerminal
{
    public bool Match(ref Scanner scanner, bool advance);
}

public record struct CharTerminalParser(char Character) : ITerminal
{
    public readonly bool Match(ref Scanner scanner, bool advance)
    {
        if(advance && scanner.Peek() == Character)
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
    public readonly bool Match(ref Scanner scanner, bool advance)
    {
        var found = (scanner.Peek(), Mode) switch
        {
            ( >= 0, DigitMode.All) => char.IsDigit((char)scanner.Peek()),
            ( >= 0, DigitMode.OnlyZero) => (char)scanner.Peek() == '0',
            ( >= 0, DigitMode.ExceptZero) => (char)scanner.Peek() != '0' && char.IsDigit((char)scanner.Peek()),
            _ => false
        };
        if(advance && found)
            scanner.Advance(1);
        return found;
    }
}

public record struct LetterTerminalParser() : ITerminal
{
    public readonly bool Match(ref Scanner scanner, bool advance)
    {
        if(scanner.Peek() > 0 && char.IsLetter((char)scanner.Peek()))
        {
            if(advance)
                scanner.Advance(1);
            return true;
        }
        return false;
    }
}
public record struct LetterOrDigitTerminalParser() : ITerminal
{
    public readonly bool Match(ref Scanner scanner, bool advance)
    {
        if(scanner.Peek() > 0 && char.IsLetterOrDigit((char)scanner.Peek()))
        {
            if(advance)
                scanner.Advance(1);
            return true;
        }
        return false;
    }
}

public record struct LiteralTerminalParser(string Literal, bool CaseSensitive = true) : ITerminal
{
    public readonly bool Match(ref Scanner scanner, bool advance)
    {
        if(scanner.ReadString(Literal, CaseSensitive))
        {
            if(advance)
                scanner.Advance(Literal.Length);
            return true;
        }
        return false;
    }
    public static implicit operator LiteralTerminalParser(string lit) => new(lit);
}

public record struct SetTerminalParser(string Set) : ITerminal
{
    public readonly bool Match(ref Scanner scanner, bool advance)
    {
        if(scanner.Peek() > 0 && Set.Contains((char)scanner.Peek()))
        {
            if(advance)
                scanner.Advance(1);
            return true;
        }
        return false;
    }

    public static implicit operator SetTerminalParser(string set) => new(set);
}

public record struct EOFTerminalParser() : ITerminal
{
    public readonly bool Match(ref Scanner scanner, bool advance)
    {
        return scanner.IsEof;
    }
}
public record struct EOLTerminalParser() : ITerminal
{
    public readonly bool Match(ref Scanner scanner, bool advance)
    {
        var position = scanner.Position;
        while(scanner.Peek() == ' ')
            scanner.Advance(1);
        var result = scanner.Peek() == '\n';
        if(!advance && result)
            scanner.Position = position;
        return result;
    }
}


