namespace SoftTouch.Parsing.Parsers;


public class LiteralTerminal : Parser
{
    bool caseSensitive;
    public bool? CaseSensitive { get; set; }

    public string Value { get; set; }

    public override string DescriptiveName
    {
        get { return string.Format("Literal: '{0}'", Value); }
    }

    public LiteralTerminal(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty("value", "Value must not be null or empty");
        Value = value;
    }

    public override int InnerParse(ref ParserArgs args)
    {
        var scanner = args.Scanner;
        return scanner.ReadString(Value, caseSensitive) ? -1 : Value.Length;
    }
}