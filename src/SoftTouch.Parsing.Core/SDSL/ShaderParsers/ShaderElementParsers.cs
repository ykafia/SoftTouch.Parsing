using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct ShaderElementParsers : IParser<ShaderElement>
{
    public bool Match(ref Scanner scanner, ParseResult result, out ShaderElement parsed, in ParseError? orError = null)
    {
        throw new NotImplementedException();
    }

    public static bool Method(ref Scanner scanner, ParseResult result, out ShaderElement parsed, in ParseError? orError = null)
        => new ShaderMethodParsers().Match(ref scanner, result, out parsed, in orError);
}