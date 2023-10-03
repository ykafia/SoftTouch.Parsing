namespace SoftTouch.Parsing.Parsers;

public class SequenceParser : ListParser
{
    public override int InnerParse(ref ParserArgs args)
    {
        foreach(var p in Parsers)
        {
            if(p.InnerParse(ref args) == -1)
                return -1;
        }
        return 1;
    }
}