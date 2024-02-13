using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct StatementParsers : IParser<Statement>
{
    public bool Match(ref Scanner scanner, ParseResult result, out Statement parsed)
    {
        throw new NotImplementedException();
    }
}