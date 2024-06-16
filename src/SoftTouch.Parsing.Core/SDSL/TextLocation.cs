using CommunityToolkit.HighPerformance.Buffers;

namespace SoftTouch.Parsing.SDSL;

public record struct TextLocation(ReadOnlyMemory<char> Original, Range Range)
{
    public ReadOnlyMemory<char> Text { get; } = Original[Range];
    public readonly ReadOnlySpan<char> TextSpan => Text.Span;
    public readonly int Length => Range.GetOffsetAndLength(Original.Length).Length;

    public readonly int Line => Original.Span[..Range.EndsAt(Original.Length)].Count('\n');
    public readonly int Column => Range.EndsAt(Original.Length) - Original.Span[..Range.EndsAt(Original.Length)].LastIndexOf('\n');
    public readonly override string ToString()
    {
        return $"[l{Line}-c{Column}]\n{Text.Span}";
    }
}

public static class SpanCharExtensions
{
    public static int Sum(this (int offset, int length) ol) => ol.offset + ol.length;

    public static int EndsAt(this Range range, int originalLength) => range.GetOffsetAndLength(originalLength).Sum();
}