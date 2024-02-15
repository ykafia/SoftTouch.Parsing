using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public readonly struct ErrorLocation
{
    public readonly ReadOnlyMemory<char> Text { get; }
    public readonly int Position { get; }
    private readonly int leftOffset;
    private readonly int rightOffset;
    private readonly int line;
    private readonly int column;
    public ErrorLocation(Scanner scanner, int position)
    {
        // Getting the line and column at the position given.
        // TODO: Make this a function in scanner
        var pos = scanner.Position;
        scanner.Position = position;
        line = scanner.Line;
        column = scanner.Column;
        scanner.Position = pos;

        // Setting other attributes
        leftOffset = position - 5 > 0 ? 5 : position;
        rightOffset = position + 5 < scanner.Code.Length ? 5 : scanner.Code.Length - position - 1;
        Position = position;
        
        Text = scanner.Memory[(position - leftOffset)..(position + rightOffset)];
    }

    public override string ToString()
    {
        return $"l{line}-c{column} : \n{Text[..5]}>>>{Text[5..]}";
    }
}


public record struct ParseError(string Message, ErrorLocation Location)
{
    public override readonly string ToString()
    {
        return $"{Message} at : {Location}";
    }
}


public class ParseResult<T>
    where T : Node
{
    public T? AST { get; set; }
    public List<ParseError> Errors { get; internal set; } = [];
}
public class ParseResult : ParseResult<Node>;