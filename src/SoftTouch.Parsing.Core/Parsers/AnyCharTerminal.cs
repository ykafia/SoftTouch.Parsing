namespace SoftTouch.Parsing.Parsers;

public class AnyCharTerminal : Parser
{

    public AnyCharTerminal()
    {
    }

    public override int InnerParse(ref ParserArgs args)
    {
        var pos = args.Scanner.Advance(1);
        return pos < 0 ? -1 : 1;
    }
}