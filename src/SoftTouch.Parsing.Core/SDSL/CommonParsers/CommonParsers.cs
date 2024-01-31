using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public static class CommonParsers
{
    public static bool Spaces0(ref Scanner scanner, ParseResult result, out NoNode node)
        => new Space0().Match(ref scanner, result, out node);
    public static bool Spaces1(ref Scanner scanner, ParseResult result, out NoNode node)
        => new Space1().Match(ref scanner, result, out node);
}