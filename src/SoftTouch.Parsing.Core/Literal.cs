namespace SoftTouch.Parsing.Core;


public readonly struct Literal
{
    public string Value { get; init; }

    public static implicit operator string(Literal l) => l.Value;
}