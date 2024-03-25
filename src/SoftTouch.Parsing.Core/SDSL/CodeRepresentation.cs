using System.Diagnostics;
using System.Text;

namespace SoftTouch.Parsing.SDSL;



public interface ICodeRepresentation : IDisposable
{
    public int Position { get; set; }
    public int Length { get; }
    public char this[Index index] { get; }
    public ReadOnlySpan<char> this[Range range] { get; }
    public ReadOnlySpan<char> Span { get; }
    public ReadOnlyMemory<char> Memory { get; }
    public ReadOnlyMemory<char> Slice(int start, int length);
}

public struct StringCode : ICodeRepresentation
{
    public ReadOnlyMemory<char> Memory { get; }
    public readonly ReadOnlySpan<char> Span => Memory.Span;
    public int Position { get; set; }

    public readonly int Length => Memory.Length;

    public readonly char this[Index index] => Span[index];

    public readonly ReadOnlySpan<char> this[Range range] => Span[range];

    public StringCode(string code)
    {
        Memory = code.AsMemory();
    }

    public StringCode(ReadOnlyMemory<char> code)
    {
        Memory = code;
    }

    public static implicit operator StringCode(string c) => new(c);
    public static implicit operator StringCode(ReadOnlyMemory<char> c) => new(c);

    public readonly void Dispose(){}

    public readonly ReadOnlyMemory<char> Slice(int start, int length) => Memory.Slice(start, length);
}

public record struct Transposition(Range Origin, Range Destination);

public struct TokenizedCode() : ICodeRepresentation
{
    public readonly char this[Index index] => Processed[index];

    public readonly ReadOnlySpan<char> this[Range range] => Processed.Span[range];

    public List<Transposition> Transpositions { get; } = [];
    public CodeBuffer Original { get; } = new();
    public PreProcessedCodeBuffer Processed { get; } = new();
    public int Position { get; set; }
    public readonly int Length => Processed.Length;

    public readonly ReadOnlySpan<char> Span => Processed.Span;

    public readonly ReadOnlyMemory<char> Memory => Processed.Memory;

    public readonly void Dispose()
    {
        Original.Dispose();
        Processed.Dispose();
    }

    public readonly ReadOnlyMemory<char> Slice(int start, int length)
        => Memory.Slice(start, length);


    public readonly void AddToken(in Range location)
    {
        Processed.Add(Original[location]);
        var (_, length) = location.GetOffsetAndLength(Original.Length);
        Transpositions.Add(new(location, new(Processed.Length - length, Processed.Length)));
    }


    public readonly override string ToString()
    {
        var builder = new StringBuilder();
        foreach(var e in Transpositions)
            builder.Append(Processed[e.Destination]);
        return builder.ToString();
    }
}