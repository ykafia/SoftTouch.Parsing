
namespace SoftTouch.Parsing.SDSL;



public interface ITerminal : IParser<TerminalResult>;

public record struct TerminalResult(
    char? Char = null, 
    string? Set = null,
    string? Literal = null
);
public record struct CharTerminal(char Character) : ITerminal
{
    public readonly bool Match(ref Scanner scanner, out TerminalResult result)
    {
        result = new();
        if (scanner.Peek() == Character)
        {
            result = new();
            return true;
        }
        else 
            return false;
    }
    public static implicit operator CharTerminal(char c) => new(c);
}


public record struct DigitTerminal() : ITerminal
{
    public readonly bool Match(ref Scanner scanner, out TerminalResult result)
    {
        result = new();
        if(scanner.Peek() > 0 && char.IsDigit((char)scanner.Peek()))
        {
            return true;
        }
        else return false;
    }
}

public record struct LetterTerminal() : ITerminal
{
    public readonly bool Match(ref Scanner scanner, out TerminalResult result)
    {
        result = new();
        if(scanner.Peek() > 0 && char.IsLetter((char)scanner.Peek()))
            return true;
        else return false;
    }
}

public record struct LiteralTerminal(string Literal, bool CaseSensitive = true) : ITerminal
{
    public readonly bool Match(ref Scanner scanner, out TerminalResult result)
    {
        result = new();
        if(scanner.ReadString(Literal, CaseSensitive))
            return true;
        else return false;
    }
    public static implicit operator LiteralTerminal(string lit) => new(lit);
}

public record struct SetTerminal(string Set) : ITerminal
{
    public readonly bool Match(ref Scanner scanner, out TerminalResult result)
    {
        result = new();
        if(scanner.Peek() > 0 && Set.Contains((char)scanner.Peek()))
            return true;
        else return false;
    }

    public static implicit operator SetTerminal(string set) => new(set);
}




