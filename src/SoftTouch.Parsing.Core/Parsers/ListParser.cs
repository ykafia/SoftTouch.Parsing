namespace SoftTouch.Parsing.Parsers;

public abstract class ListParser : Parser
{
    public List<Parser> Parsers {get; set;}

    public ListParser(params Parser[] parsers)
    {
        Parsers = new();
        Parsers.AddRange(parsers);
    }
}