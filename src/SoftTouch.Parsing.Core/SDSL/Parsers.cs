namespace SoftTouch.Parsing.SDSL;

public interface IParser;

public interface IParser<TResult>
{
    public bool Match(ref Scanner scanner, out TResult result);
}
public struct NumberResult
{
    public double? DoubleValue { get; set; }
    public long? LongValue { get; set; }
    public readonly bool IsDecimal => DoubleValue != null;
    public bool IsHexNotation { get; set; }

}



public struct NumberParser : IParser<NumberResult>
{
    public bool Match(ref Scanner scanner, out NumberResult result)
    {
        throw new NotImplementedException();
    }
}