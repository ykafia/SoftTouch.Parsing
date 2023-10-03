namespace SoftTouch.Parsing;


public abstract class Parser
{
    public string? Name { get; set; }
    public virtual string? DescriptiveName { get; set; }
    public bool Store => Name is not null;
    public abstract int InnerParse(ref ParserArgs scanner);
}