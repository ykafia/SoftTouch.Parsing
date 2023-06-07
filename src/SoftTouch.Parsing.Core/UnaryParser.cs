namespace SoftTouch.Parsing.Core;


public class LiteralParser : Parser
{
    Literal literal;

    public LiteralParser(Literal lit)
    {
        literal = lit;
    }

    public override bool ParseInternal(ref StringScanner scanner)
    {
        scanner.ReadString(literal);
    }
}


public class AlternativeParser : Parser
{
    List<Parser> Internals;

    public AlternativeParser()
    {
        Internals = new();
    }
    public AlternativeParser(params Parser[] parsers)
    {
        Internals = parsers.ToList();
    }
    public override bool ParseInternal(ref StringScanner scanner)
    {
        throw new NotImplementedException();
    }
}