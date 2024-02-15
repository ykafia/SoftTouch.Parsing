using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public static class CommonParsers
{
    public static bool Spaces0(ref Scanner scanner, ParseResult result, out NoNode node, in ParseError? orError = null)
        => new Space0().Match(ref scanner, result, out node, in orError);
    public static bool Spaces1(ref Scanner scanner, ParseResult result, out NoNode node, in ParseError? orError = null)
        => new Space1().Match(ref scanner, result, out node, in orError);
}