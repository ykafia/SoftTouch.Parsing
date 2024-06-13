using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct ShaderElementParsers : IParser<ShaderElement>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ShaderElement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        if(Method(ref scanner, result, out parsed))
            return true;
        else return false;
    }
    public static bool ShaderElement<TScanner>(ref TScanner scanner, ParseResult result, out ShaderElement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
        => new ShaderElementParsers().Match(ref scanner, result, out parsed, in orError);

    public static bool Method<TScanner>(ref TScanner scanner, ParseResult result, out ShaderElement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
        => new ShaderMethodParsers().Match(ref scanner, result, out parsed, in orError);
}