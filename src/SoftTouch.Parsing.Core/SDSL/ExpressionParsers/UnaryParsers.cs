using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct UnaryParsers
{
    internal static bool Not(ref Scanner scanner, ParseResult result, out Expression cast)
        => new NotExpressionParser().Match(ref scanner, result, out cast);
    internal static bool Signed(ref Scanner scanner, ParseResult result, out Expression cast)
        => new SignExpressionParser().Match(ref scanner, result, out cast);
    internal static bool PrefixIncrement(ref Scanner scanner, ParseResult result, out Expression cast)
        => new PrefixIncrementParser().Match(ref scanner, result, out cast);
    internal static bool Cast(ref Scanner scanner, ParseResult result, out Expression cast)
        => new CastExpressionParser().Match(ref scanner, result, out cast);
    public static bool Prefix(ref Scanner scanner, ParseResult result, out Expression prefix)
        => new PrefixParser().Match(ref scanner, result, out prefix);
    public static bool Postfix(ref Scanner scanner, ParseResult result, out Expression postfix)
       => new PostfixParser().Match(ref scanner, result, out postfix);
}
