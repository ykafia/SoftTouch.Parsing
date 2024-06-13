namespace SoftTouch.Parsing.SDSL;

public interface IScannableCode
{
    ReadOnlySpan<char> Span { get; }
    ReadOnlyMemory<char> Memory { get; }
}




