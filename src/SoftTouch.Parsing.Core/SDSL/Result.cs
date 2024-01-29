using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public class ParseError(string message, TextLocation location)
{
    public string Message { get; set; } = message;
    public TextLocation Location { get; set; } = location;

    public override string ToString()
    {
        return $"{Message} at : {Location}";
    }
}


public class ParseResult<T>
    where T : Node
{
    public T? AST { get; set; }
    public List<ParseError> Errors { get; } = [];
}
public class ParseResult : ParseResult<Node>
{
    public Node? AST { get; set; }
    public List<ParseError> Errors { get; } = [];
}