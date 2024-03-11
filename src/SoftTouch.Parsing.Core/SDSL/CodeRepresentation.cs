namespace SoftTouch.Parsing.SDSL;


public interface ICodeRepresentation
{
    public int Position { get; set; }
    public int Length { get; }
    public char this[Index index] { get; }
    public ReadOnlySpan<char> this[Range range] { get; }
}

public struct StringCode(string code) : ICodeRepresentation
{
    public ReadOnlyMemory<char> Memory { get; } = code.AsMemory();
    public readonly ReadOnlySpan<char> Span => Memory.Span;
    public int Position { get; set; }

    public readonly int Length => Memory.Length;

    public readonly char this[Index index] => Span[index];

    public readonly ReadOnlySpan<char> this[Range range] => Span[range];
}

public record struct CodeToken(int Start, int Length, int Line, int Column);


public struct TokenizedCode(List<CodeToken> tokens) : ICodeRepresentation
{
    public ReadOnlyMemory<char> Memory { get; } = code.AsMemory();
    public readonly ReadOnlySpan<char> Span => Memory.Span;
    public int Position { get; set; }

    public readonly int Length => Memory.Length;

    public readonly char this[Index index] => Span[index];

    public readonly ReadOnlySpan<char> this[Range range] => Span[range];
}