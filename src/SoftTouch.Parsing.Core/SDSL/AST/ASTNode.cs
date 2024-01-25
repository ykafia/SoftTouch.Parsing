namespace SoftTouch.Parsing.SDSL.AST;



public record struct TextLocation(int Line, int Column, ReadOnlyMemory<char> Memory)
{
    public readonly int Length => Memory.Length;
}


public abstract class Node(TextLocation info)
{
    public TextLocation Info { get; set; } = info;
}