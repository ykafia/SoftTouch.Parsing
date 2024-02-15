namespace SoftTouch.Parsing.SDSL;

public record struct TextLocation(int Line, int Column, ReadOnlyMemory<char> Text)
{
    public readonly int Length => Text.Length;
    public readonly override string ToString()
    {
        return $"l{Line} c{Column}, {Text.Span}";
    }
}