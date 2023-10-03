namespace SoftTouch.Parsing;


public ref struct ParserArgs
{
    public MatchList Matches { get; private set; }
    public StringScanner Scanner { get; set; }

    public ParserArgs(string text)
    {
        Matches = new MatchList(text);
        Scanner = new StringScanner(text);
    }
}