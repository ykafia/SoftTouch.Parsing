namespace SoftTouch.Parsing.Parsers;


public class Terminals
{
    public static DigitTerminal Digit() => new();
    public static LetterTerminal Letter() => new();
    public static LetterOrDigitTerminal LetterOrDigit() => new();
    public static WhiteSpaceTerminal WhiteSpace() => new();
    public static AnyCharTerminal AnyChar() => new();
    public static LiteralTerminal Literal(string value) => new(value);

}