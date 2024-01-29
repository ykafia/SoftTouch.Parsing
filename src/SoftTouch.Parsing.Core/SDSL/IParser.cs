using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public interface IParser;

public interface IParser<TResult>
    where TResult : Node
{
    public bool Match(ref Scanner scanner, ParseResult result, out TResult parsed);
}