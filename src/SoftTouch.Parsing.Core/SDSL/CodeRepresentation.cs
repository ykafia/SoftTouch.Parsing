using System.Diagnostics;

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

    public void Dispose(){}

    public ReadOnlyMemory<char> Slice(int start, int length) => Memory.Slice(start, length);
}

public record struct CodeToken(int Start, int Length, int Line, int Column);


public struct TokenizedCode : ICodeRepresentation
{
    public PreProcessedCodeBuffer Processed { get; set; }
    public readonly ReadOnlySpan<char> Span => Processed.Span;
    public readonly ReadOnlyMemory<char> Memory => Processed.Memory;
    public int Position { get; set; }
    public readonly int Length => Processed.Length;

    public readonly char this[Index index] => Span[index];

    public readonly ReadOnlySpan<char> this[Range range] => Span[range];
    public void Dispose() 
    {
        Processed.Dispose();
    }
    public readonly ReadOnlyMemory<char> Slice(int start, int length) => Memory.Slice(start, length);


}