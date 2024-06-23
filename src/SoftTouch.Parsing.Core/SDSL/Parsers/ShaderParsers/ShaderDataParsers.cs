using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public record struct ShaderDataParsers : IParser<ShaderElement>
{
    public bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ShaderElement parsed, in ParseError? orError = null) where TScanner : struct, IScanner
    {
        throw new NotImplementedException();
    }
}

public record struct ShaderFieldParser : IParser<ShaderElement>
{
    public bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ShaderElement parsed, in ParseError? orError = null) where TScanner : struct, IScanner
    {
        var position = scanner.Position;
        var isStage = false;
        var isStream = false;
        if (Terminals.Literal("stage", ref scanner, advance: true) && CommonParsers.Spaces1(ref scanner, result, out _))
            isStage = true;
        if (Terminals.Literal("stream", ref scanner, advance: true) && CommonParsers.Spaces1(ref scanner, result, out _))
            isStream = true;
        if (LiteralsParser.Identifier(ref scanner, result, out Identifier typename) 
            && CommonParsers.Spaces1(ref scanner, result, out _)
            && LiteralsParser.Identifier(ref scanner, result, out Identifier name) 
            && CommonParsers.FollowedBy(ref scanner, Terminals.Set("=;"), withSpaces : true)
        )
        {
            var field = new ShaderField(typename, name, isStream, isStage, null, scanner.GetLocation(position, scanner.Position - position));
            
        }
        

    }
}