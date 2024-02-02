using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public record struct PostfixParser : IParser<PostfixExpression>
{
    public bool Match(ref Scanner scanner, ParseResult result, out PostfixExpression parsed)
    {
        throw new NotImplementedException();
    }
}