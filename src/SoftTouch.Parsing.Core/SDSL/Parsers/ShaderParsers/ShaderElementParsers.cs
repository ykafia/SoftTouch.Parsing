using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct ShaderElementParsers : IParser<ShaderElement>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ShaderElement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        if(BufferParsers.Buffer(ref scanner, result, out var buffer, orError))
        {
            parsed = buffer;
            return true;
        }
        else if(ShaderMemberParser.Member(ref scanner, result, out var member, orError))
        {
            parsed = member;
            return true;
        }
        else if(Method(ref scanner, result, out parsed))
            return true;
        else return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }
    public static bool ShaderElement<TScanner>(ref TScanner scanner, ParseResult result, out ShaderElement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
        => new ShaderElementParsers().Match(ref scanner, result, out parsed, in orError);

    public static bool Method<TScanner>(ref TScanner scanner, ParseResult result, out ShaderElement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
        => new ShaderMethodParsers().Match(ref scanner, result, out parsed, in orError);
}