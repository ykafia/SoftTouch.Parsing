using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct UnaryParsers
{
    internal static bool Not(ref Scanner scanner, ParseResult result, out Expression cast, in ParseError? orError = null)
        => new NotExpressionParser().Match(ref scanner, result, out cast, in orError);
    internal static bool Signed(ref Scanner scanner, ParseResult result, out Expression cast, in ParseError? orError = null)
        => new SignExpressionParser().Match(ref scanner, result, out cast, in orError);
    internal static bool PrefixIncrement(ref Scanner scanner, ParseResult result, out Expression cast, in ParseError? orError = null)
        => new PrefixIncrementParser().Match(ref scanner, result, out cast, in orError);
    internal static bool Cast(ref Scanner scanner, ParseResult result, out Expression cast, in ParseError? orError = null)
        => new CastExpressionParser().Match(ref scanner, result, out cast, in orError);
    public static bool Prefix(ref Scanner scanner, ParseResult result, out Expression prefix, in ParseError? orError = null)
        => new PrefixParser().Match(ref scanner, result, out prefix, in orError);
    public static bool Postfix(ref Scanner scanner, ParseResult result, out Expression postfix, in ParseError? orError = null)
       => new PostfixParser().Match(ref scanner, result, out postfix, in orError);
}
