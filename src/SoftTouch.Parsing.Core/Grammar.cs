namespace SoftTouch.Parsing;

public class Grammar
{
    public Parser Inner { get; set; }

    public Grammar(Parser inner)
    {
        Inner = inner;
    }

    public MatchList Match(string text)
    {
        var args = new ParserArgs(text);
        Inner.InnerParse(ref args);
        return args.Matches;
    }
}