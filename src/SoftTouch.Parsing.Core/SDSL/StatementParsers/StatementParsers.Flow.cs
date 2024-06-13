using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;



public record struct IfStatementParser : IParser<Statement>
{
    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out Statement parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
    {
        throw new NotImplementedException();
        // var position = scanner.Position;
        // if (
        //     Terminals.Literal("if", ref scanner, advance: true)
        //     && CommonParsers.Spaces0(ref scanner, result, out _)
        // )
        // {
        //     if(Terminals.Char('(', ref scanner, advance:true) && CommonParsers.Spaces0(ref scanner, result, out _))
        //     {
        //         if(ExpressionParser.Expression(ref scanner, result, out var cond) && CommonParsers.Spaces0(ref scanner, result, out _))
        //         {

        //         }
        //     }
        //     else
        //     {
        //         parsed = null!;
        //         result.Errors.Add(new("Expected parenthesis", scanner.CreateError(scanner.Position)));
        //         return false;
        //     }

        // }
        // else
        // {
        //     scanner.Position = position;
        //     parsed = null!;
        //     return false;
        // }
    }
}

