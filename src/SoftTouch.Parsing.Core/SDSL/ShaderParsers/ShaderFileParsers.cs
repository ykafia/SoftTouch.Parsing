using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;




public record struct ShaderFileParser : IParser<ShaderFile>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out ShaderFile parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        CommonParsers.Spaces0(ref scanner, result, out _);
        var file = new ShaderFile(new(0,0,scanner.Memory));
        while (!scanner.IsEof)
        {
            if (
                Terminals.Literal("namespace", ref scanner) 
                && NamespaceParsers.Namespace(ref scanner, result, out var ns)
            )
            {
                file.Namespaces.Add(ns);
                CommonParsers.Spaces0(ref scanner, result, out _);
            }
            else if(Terminals.Literal("shader", ref scanner)
                && ShaderClassParsers.Class(ref scanner, result, out var shader)
            )
            {
                file.RootClasses.Add(shader);
                CommonParsers.Spaces0(ref scanner, result, out _);
            }
        }
        parsed = file;
        return true;
    }
}




public record struct NamespaceParsers : IParser<ShaderNamespace>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out ShaderNamespace parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        if (
            Terminals.Literal("namespace", ref scanner, advance: true)
            && CommonParsers.Spaces1(ref scanner, result, out _)
        )
        {
            var ns = new ShaderNamespace(new());
            do
            {
                if (LiteralsParser.Identifier(ref scanner, result, out var identifier))
                    ns.NamespacePath.Add(identifier);
                else 
                {
                    result.Errors.Add(new("Expected identifier", new(scanner,scanner.Position)));
                    scanner.Position = scanner.Span.Length;
                }
                CommonParsers.Spaces0(ref scanner, result, out _);
            }
            while (!scanner.IsEof && Terminals.Char('.', ref scanner, advance: true));
            if(ns.NamespacePath.Count > 0)
                ns.Namespace = string.Join(".", ns.NamespacePath);
            if (Terminals.Char(';', ref scanner, advance: true))
            {
                CommonParsers.Spaces0(ref scanner, result, out _);
                while (ShaderClassParsers.Class(ref scanner, result, out var shader))
                {
                    ns.ShaderClasses.Add(shader);
                }
            }
            else if (Terminals.Char('{', ref scanner, advance: true))
            {
                CommonParsers.Spaces0(ref scanner, result, out _);
                while (!scanner.IsEof && !Terminals.Char('}', ref scanner, advance: true))
                {
                    if(ShaderClassParsers.Class(ref scanner, result, out var shader) && CommonParsers.Spaces0(ref scanner, result, out _))
                        ns.ShaderClasses.Add(shader);
                    else 
                    {
                        result.Errors.Add(new("Expected shader class", new(scanner, scanner.Position)));
                        scanner.Position = scanner.Span.Length;
                        parsed = null!;
                        return false;
                    }
                }
            }
            else
            {
                scanner.Position = position;
                parsed = null!;
                return false;
            }
            ns.Info = scanner.GetLocation(position, scanner.Position - position);
            parsed = ns;
            return true;
        }
        parsed = null!;
        return false;
    }

    public static bool Namespace(ref Scanner scanner, ParseResult result, out ShaderNamespace parsed, in ParseError? orError = null)
        => new NamespaceParsers().Match(ref scanner, result, out parsed, orError);
}