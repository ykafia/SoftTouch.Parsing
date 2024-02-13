using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public readonly struct ErrorLocation
{
    public readonly ReadOnlyMemory<char> Text { get; }
    public readonly int Position { get; }
    private readonly int leftOffset;
    private readonly int rightOffset;
    public ErrorLocation(Scanner scanner, int position)
    {
        leftOffset = position - 5 > 0 ? 5 : position;
        rightOffset = position + 5 < scanner.Code.Length ? 5 : scanner.Code.Length - position - 1;
        Position = position;
        Text = scanner.Memory[(position - leftOffset)..(position + rightOffset)];
    }

    public override string ToString()
    {
        return $"{Text[..5]}>>>{Text[5..]}";
    }
}


public class ParseError(string message, ErrorLocation location)
{
    public string Message { get; set; } = message;
    public ErrorLocation Location { get; set; } = location;

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