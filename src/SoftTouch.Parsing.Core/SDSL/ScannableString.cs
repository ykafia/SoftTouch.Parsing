
namespace SoftTouch.Parsing.SDSL;


public readonly struct ScannableString(string code) : IScannableCode
{
    public string Code { get; } = code;
    public readonly ReadOnlySpan<char> Span => Code.AsSpan();
    public readonly ReadOnlyMemory<char> Memory => Code.AsMemory();

    public static implicit operator ScannableString(string s) => new (s);
    public static implicit operator string(ScannableString s) => s.Code;
}

public readonly struct ScannableMemory(ReadOnlyMemory<char> code) : IScannableCode
{
    public ReadOnlyMemory<char> Code { get; } = code;
    public readonly ReadOnlySpan<char> Span => Code.Span;
    public readonly ReadOnlyMemory<char> Memory => Code;

    public static implicit operator ScannableMemory(ReadOnlyMemory<char> s) => new(s);
    public static implicit operator ReadOnlyMemory<char>(ScannableMemory s) => s.Code;
}