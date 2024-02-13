using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public readonly struct ErrorLocation
{
    public readonly ReadOnlyMemory<char> Text { get; }
    public readonly int Position { get; }
    private readonly int offset;
    private readonly int rightOffset;
    public ErrorLocation(Scanner scanner, int position)
    {
        offset = Math.Max(-5, -position);
        rightOffset = Math.Min(position + 5, scanner.Code.Length - 1);
        Position = position;
        Text = scanner.Memory[offset..rightOffset];
    }

    public override string ToString()
    {
        return $"{Text[..5]}>>>{Text[5..]}";
    }
}


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