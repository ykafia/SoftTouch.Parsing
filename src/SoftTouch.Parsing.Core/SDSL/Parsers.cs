using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public interface IParser;

public interface IParser<TResult>
{
    public bool Match(ref Scanner scanner, out TResult result);
}
public struct NumberResult
{
    public double? DoubleValue { get; set; }
    public long? LongValue { get; set; }
    public readonly bool IsDecimal => DoubleValue != null;
    public bool IsHexNotation { get; set; }

}



public struct IntegerParser : IParser<IntegerLiteral>
{
    public readonly bool Match(ref Scanner scanner, out IntegerLiteral result)
    {
        var position = scanner.Position;
        DigitTerminalParser zero = new(DigitMode.OnlyZero);
        DigitTerminalParser nonZero = new(DigitMode.ExceptZero);
        DigitTerminalParser digitParser = new();
        LetterTerminalParser suffix = new();
        if(nonZero.Match(ref scanner, out _))
        {
            scanner.Advance(1);
            while(digitParser.Match(ref scanner, out _))
                scanner.Advance(1);
            if(suffix.Match(ref scanner, out _))
            {
                result = null!;
                return false;
            }
            else
            {
                var memory = scanner.Memory[position..scanner.Position];
                result = new(32, long.Parse(memory.Span), new(scanner.Line, scanner.Column - memory.Length, memory));
                return true;
            }
        }
        result = new(0,0,new());
        return false;
    }
}

public struct FloatParser : IParser<FloatLiteral>
{
    public bool Match(ref Scanner scanner, out FloatLiteral result)
    {
        var position = scanner.Position;
        DigitTerminalParser zero = new(DigitMode.OnlyZero);
        DigitTerminalParser nonZero = new(DigitMode.ExceptZero);
        DigitTerminalParser digitParser = new();
        CharTerminalParser dot = new('.');
        SetTerminalParser suffix = new("fdh");
        if (dot.Match(ref scanner, out _))
        {
            scanner.Advance(1);
            while (digitParser.Match(ref scanner, out _))
                scanner.Advance(1);
        }
        else if (nonZero.Match(ref scanner, out _))
        {
            scanner.Advance(1);
            bool hasDot = true;

        }
        throw new NotImplementedException();
    }
}
public struct HexParser : IParser<HexLiteral>
{
    public bool Match(ref Scanner scanner, out HexLiteral result)
    {
        var position = scanner.Position;
        DigitTerminalParser zero = new(DigitMode.OnlyZero);
        DigitTerminalParser nonZero = new(DigitMode.ExceptZero);
        DigitTerminalParser digitParser = new();
        CharTerminalParser dot = new('.');
        SetTerminalParser suffix = new("fdh");
        if (dot.Match(ref scanner, out _))
        {
            scanner.Advance(1);
            while (digitParser.Match(ref scanner, out _))
                scanner.Advance(1);
        }
        else if (nonZero.Match(ref scanner, out _))
        {
            scanner.Advance(1);
            bool hasDot = true;

        }
        throw new NotImplementedException();
    }
}