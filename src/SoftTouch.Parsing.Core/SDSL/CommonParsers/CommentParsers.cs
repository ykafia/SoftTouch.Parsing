using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct CodeNodeParsers : IParser<CodeSnippets>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out CodeSnippets parsed, in ParseError? error = null)
    {
        parsed = new();
        while(!scanner.IsEof)
        {
            var position = scanner.Position;
            while(!(Terminals.Literal("/*", ref scanner) || Terminals.Literal("//", ref scanner) || scanner.IsEof))
                scanner.Advance(1);
            parsed.Snippets.Add(new(scanner.GetLocation(position, scanner.Position - position)));
            if (Terminals.Literal("/*", ref scanner, advance: true))
                while (!(Terminals.Literal("*/", ref scanner, advance: true) || scanner.IsEof))
                    scanner.Advance(1);
            else if (Terminals.Literal("//", ref scanner, advance: true))
                while (!(Terminals.Char('\n', ref scanner, advance: true) || scanner.IsEof))
                    scanner.Advance(1);
        }
        return true;
    }
}