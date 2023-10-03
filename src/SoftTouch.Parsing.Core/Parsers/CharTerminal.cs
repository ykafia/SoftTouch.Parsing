namespace SoftTouch.Parsing.Parsers;


public abstract class CharTerminal : Parser, IInverseParser
{
    public bool Inverse { get; set; }

    public abstract bool Test(char c);

    public override int InnerParse(ref ParserArgs args)
    {
        var scanner = args.Scanner;
        int ch = scanner.ReadChar();
        if (ch != -1)
        {
            if (Test((char)ch) != Inverse)
            {
                return 1;
            }
            scanner.Position -= 1;
        }
        return -1;
    }
}

public class DigitTerminal : CharTerminal
{
    public override bool Test(char c)
    {
        return char.IsDigit(c);
    }
}

public class LetterTerminal : CharTerminal
{
    public override bool Test(char c)
    {
        return char.IsLetter(c);
    }
}

public class LetterOrDigitTerminal : CharTerminal
{
    public override bool Test(char c)
    {
        return char.IsLetterOrDigit(c);
    }
}
public class WhiteSpaceTerminal : CharTerminal
{
    public override bool Test(char c)
    {
        return char.IsWhiteSpace(c);
    }
}

public class SingleLineWhiteSpaceTerminal : CharTerminal
{
    public override bool Test(char c)
    {
        return c != '\n' && c != '\r' && char.IsWhiteSpace(c);
    }
}