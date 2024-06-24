using System.Runtime;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public record struct ShaderMemberParser : IParser<ShaderMember>
{
    public static bool Member<TScanner>(ref TScanner scanner, ParseResult result, out ShaderMember parsed, in ParseError? orError = null)
        where TScanner : struct, IScanner
        => new ShaderMemberParser().Match(ref scanner, result, out parsed, in orError);

    public readonly bool Match<TScanner>(ref TScanner scanner, ParseResult result, out ShaderMember parsed, in ParseError? orError = null) where TScanner : struct, IScanner
    {
        parsed = null!;
        var position = scanner.Position;
        var isStage = false;
        var isStream = false;
        if (Terminals.Literal("stage ", ref scanner, advance: true))
            isStage = true;
        CommonParsers.Spaces0(ref scanner, result, out _);
        if (Terminals.Literal("stream ", ref scanner, advance: true))
            isStream = true;
        CommonParsers.Spaces0(ref scanner, result, out _);
        if (LiteralsParser.TypeName(ref scanner, result, out var typename)
            && CommonParsers.Spaces1(ref scanner, result, out _)
            && LiteralsParser.Identifier(ref scanner, result, out Identifier name)
            && CommonParsers.FollowedBy(ref scanner, Terminals.Set("=;"), withSpaces: true)
        )
        {
            if (CommonParsers.FollowedBy(ref scanner, Terminals.Set(";"), withSpaces: true))
            {
                CommonParsers.Until(ref scanner, ';', advance: true);
                parsed = new ShaderMember(typename, name, null, scanner.GetLocation(position..scanner.Position), isStage, isStream);
                return true;
            }
            else if (CommonParsers.FollowedBy(ref scanner, Terminals.Set("="), withSpaces: true))
            {
                CommonParsers.Until(ref scanner, '=', advance: true);
                CommonParsers.Spaces0(ref scanner, result, out _);
                if (ExpressionParser.Expression(ref scanner, result, out var expression, orError: orError ?? new("Expected expression here", scanner.CreateError(scanner.Position))))
                {
                    if (CommonParsers.FollowedBy(ref scanner, Terminals.Char(':')))
                    {
                        CommonParsers.Until(ref scanner, ':', advance: true);
                        CommonParsers.Spaces0(ref scanner, result, out _);
                        if (LiteralsParser.Identifier(ref scanner, result, out var semantic, orError ?? new("Expected semantic here", scanner.CreateError(scanner.Position))))
                        {
                            if (CommonParsers.Spaces0(ref scanner, result, out _) && Terminals.Char(';', ref scanner))
                            {
                                parsed = new ShaderMember(typename, name, expression, scanner.GetLocation(position..scanner.Position), isStage, isStream, semantic);
                                return true;
                            }
                            else return CommonParsers.Exit(ref scanner, result, out parsed, position, new("Missing semi colon here", scanner.CreateError(scanner.Position)));
                                
                        }
                        else return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
                    }
                    parsed = new ShaderMember(typename, name, expression, scanner.GetLocation(position..scanner.Position), isStage, isStream);
                    return true;
                }
            }
        }
        return CommonParsers.Exit(ref scanner, result, out parsed, position, orError);
    }
}

