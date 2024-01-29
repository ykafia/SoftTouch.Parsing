
namespace SoftTouch.Parsing.SDSL;


public static class Terminals
{
    public static bool AnyChar(ref Scanner scanner) => !scanner.IsEof;
    public static bool Char(char c, ref Scanner scanner)  => new CharTerminalParser(c).Match(ref scanner);
    public static bool Literal(string c, ref Scanner scanner) => new LiteralTerminalParser(c).Match(ref scanner);
    public static bool Digit(ref Scanner scanner, DigitMode mode = DigitMode.All) => new DigitTerminalParser(mode).Match(ref scanner);
    public static bool Letter(ref Scanner scanner) => new LetterTerminalParser().Match(ref scanner);
    public static bool LetterOrDigit(ref Scanner scanner) => new LetterOrDigitTerminalParser().Match(ref scanner);
    public static bool EOL(ref Scanner scanner) => new EOLTerminalParser().Match(ref scanner);
    public static bool EOF(ref Scanner scanner) => new EOFTerminalParser().Match(ref scanner);
}

public interface ITerminal
{
    public bool Match(ref Scanner scanner);
}

public record struct CharTerminalParser(char Character)
{
    public readonly bool Match(ref Scanner scanner)
    {
        return scanner.Peek() == Character;
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
    public readonly bool Match(ref Scanner scanner)
    {
        return (scanner.Peek(), Mode) switch
        {
            ( >= 0, DigitMode.All) => char.IsDigit((char)scanner.Peek()),
            ( >= 0, DigitMode.OnlyZero) => (char)scanner.Peek() == '0',
            ( >= 0, DigitMode.ExceptZero) => (char)scanner.Peek() != '0' && char.IsDigit((char)scanner.Peek()),
            _ => false
        };
    }
}

public record struct LetterTerminalParser() : ITerminal
{
    public readonly bool Match(ref Scanner scanner)
    {
        return scanner.Peek() > 0 && char.IsLetter((char)scanner.Peek());
    }
}
public record struct LetterOrDigitTerminalParser() : ITerminal
{
    public readonly bool Match(ref Scanner scanner)
    {
        return scanner.Peek() > 0 && char.IsLetterOrDigit((char)scanner.Peek());
    }
}

public record struct LiteralTerminalParser(string Literal, bool CaseSensitive = true) : ITerminal
{
    public readonly bool Match(ref Scanner scanner)
    {
        return scanner.ReadString(Literal, CaseSensitive);
    }
    public static implicit operator LiteralTerminalParser(string lit) => new(lit);
}

public record struct SetTerminalParser(string Set) : ITerminal
{
    public readonly bool Match(ref Scanner scanner)
    {
        return scanner.Peek() > 0 && Set.Contains((char)scanner.Peek());
    }

    public static implicit operator SetTerminalParser(string set) => new(set);
}

public record struct EOFTerminalParser() : ITerminal
{
    public readonly bool Match(ref Scanner scanner)
    {
        return scanner.IsEof;
    }
}
public record struct EOLTerminalParser() : ITerminal
{
    public readonly bool Match(ref Scanner scanner)
    {
        return (char)scanner.Peek() == '\n';
    }
}


