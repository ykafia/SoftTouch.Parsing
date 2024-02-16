using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct ShaderElementParsers : IParser<ShaderElement>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out ShaderElement parsed, in ParseError? orError = null)
    {
        if(Method(ref scanner, result, out parsed))
            return true;
        else return false;
    }
    public static bool ShaderElement(ref Scanner scanner, ParseResult result, out ShaderElement parsed, in ParseError? orError = null)
        => new ShaderElementParsers().Match(ref scanner, result, out parsed, in orError);

    public static bool Method(ref Scanner scanner, ParseResult result, out ShaderElement parsed, in ParseError? orError = null)
        => new ShaderMethodParsers().Match(ref scanner, result, out parsed, in orError);
}