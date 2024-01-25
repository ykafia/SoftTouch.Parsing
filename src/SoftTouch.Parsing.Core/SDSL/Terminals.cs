
namespace SoftTouch.Parsing.SDSL;



public interface ITerminal<TResult> : IParser<TResult>;

public record struct TerminalResult(
    char? Char = null, 
    string? Set = null,
    string? Literal = null
);

public record struct CharTerminal(char Char);
public record struct LiteralTerminal(string Char);

public record struct CharTerminalParser(char Character) : ITerminal<CharTerminal>
{
    public readonly bool Match(ref Scanner scanner, out CharTerminal result)
    {
        result = new();
        if (scanner.Peek() == Character)
        {
            result = new(Char: Character);
            return true;
        }
        else 
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

public record struct DigitTerminalParser(DigitMode Mode) : ITerminal<CharTerminal>
{
    public readonly bool Match(ref Scanner scanner, out CharTerminal result)
    {
        result = new();
        if(scanner.Peek() > 0 && char.IsDigit((char)scanner.Peek()))
        {
            var c = (char)scanner.Peek();
            if(
                Mode == DigitMode.ExceptZero && c == '0'
                || Mode == DigitMode.OnlyZero && c != '0'
            )
                return false;
            result = new((char)scanner.Peek());
            return true;
        }
        else return false;
    }
}

public record struct LetterTerminalParser() : ITerminal<CharTerminal>
{
    public readonly bool Match(ref Scanner scanner, out CharTerminal result)
    {
        result = new();
        if(scanner.Peek() > 0 && char.IsLetter((char)scanner.Peek()))
        {
            result = new((char)scanner.Peek());
            return true;
        }
        else return false;
    }
}

public record struct LiteralTerminalParser(string Literal, bool CaseSensitive = true) : ITerminal<LiteralTerminal>
{
    public readonly bool Match(ref Scanner scanner, out LiteralTerminal result)
    {
        result = new();
        if(scanner.ReadString(Literal, CaseSensitive))
        {
            result = new(Literal);
            return true;
        }
        else return false;
    }
    public static implicit operator LiteralTerminalParser(string lit) => new(lit);
}

public record struct SetTerminalParser(string Set) : ITerminal<CharTerminal>
{
    public readonly bool Match(ref Scanner scanner, out CharTerminal result)
    {
        result = new();
        if(scanner.Peek() > 0 && Set.Contains((char)scanner.Peek()))
        {
            result = new((char)scanner.Peek());
            return true;
        }
        else return false;
    }

    public static implicit operator SetTerminalParser(string set) => new(set);
}



