using Microsoft.VisualBasic;
using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public record struct PreprocessorParser : IParser<PreProcessableCode>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out PreProcessableCode parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        var p = new PreProcessableCode(new());
        while (!scanner.IsEof && DirectiveStatementParsers.Statement(ref scanner, result, out var statement))
            p.Snippets.Add(statement);
        p.Info = scanner.GetLocation(position, scanner.Position - position);
        parsed = p;
        return true;
    }

    public static bool PreCode(ref Scanner scanner, ParseResult result, out PreProcessableCode parsed, in ParseError? orError = null)
        => new PreprocessorParser().Match(ref scanner, result, out parsed, orError);
}

public record struct DirectiveStatementParsers : IParser<DirectiveStatement>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out DirectiveStatement parsed, in ParseError? orError = null)
    {
        parsed = null!;
        if (Conditional(ref scanner, result, out var conditional))
        {
            parsed = conditional;
            return true;
        }
        else if (Code(ref scanner, result, out var code))
        {
            parsed = code;
            return true;
        }
        else return false;
    }

    public static bool AnyIf(ref Scanner scanner, ParseResult result, out IfDirective parsed, in ParseError? orError = null)
    {
        if (If(ref scanner, result, out var ifDirective))
        {
            parsed = ifDirective;
            return true;
        }
        else if (IfDef(ref scanner, result, out var ifdefDirective))
        {
            parsed = ifdefDirective;
            return true;
        }
        else if (IfNDef(ref scanner, result, out var ifndefDirective))
        {
            parsed = ifndefDirective;
            return true;
        }
        parsed = null!;
        return false;
    }

    public static bool If(ref Scanner scanner, ParseResult result, out IfDirective parsed, in ParseError? orError = null)
        => new ConditionalIfDirectivesParser().Match(ref scanner, result, out parsed, orError);
    public static bool IfDef(ref Scanner scanner, ParseResult result, out IfDefDirective parsed, in ParseError? orError = null)
        => new ConditionalIfDefDirectivesParser().Match(ref scanner, result, out parsed, orError);
    public static bool IfNDef(ref Scanner scanner, ParseResult result, out IfNDefDirective parsed, in ParseError? orError = null)
        => new ConditionalIfNDefDirectivesParser().Match(ref scanner, result, out parsed, orError);
    public static bool Elif(ref Scanner scanner, ParseResult result, out IfDirective parsed, in ParseError? orError = null)
        => new ConditionalElifDirectivesParser().Match(ref scanner, result, out parsed, orError);
    public static bool Endif(ref Scanner scanner, ParseResult result, in ParseError? orError = null)
        => new EndifDirectiveParser().Match(ref scanner, result, out _, orError);
    public static bool Code(ref Scanner scanner, ParseResult result, out DirectiveCode parsed, in ParseError? orError = null)
        => new DirectiveCodeParser().Match(ref scanner, result, out parsed, orError);
    public static bool Else(ref Scanner scanner, ParseResult result, out ElseDirective parsed, in ParseError? orError = null)
        => new ConditionalElseDirectivesParser().Match(ref scanner, result, out parsed, orError);
    public static bool Conditional(ref Scanner scanner, ParseResult result, out ConditionalDirectives parsed, in ParseError? orError = null)
        => new ConditionalDirectivesParser().Match(ref scanner, result, out parsed, orError);
    public static bool Statement(ref Scanner scanner, ParseResult result, out DirectiveStatement parsed, in ParseError? orError = null)
        => new DirectiveStatementParsers().Match(ref scanner, result, out parsed, orError);
}

public struct ConditionalDirectivesParser : IParser<ConditionalDirectives>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out ConditionalDirectives parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;

        if (DirectiveStatementParsers.AnyIf(ref scanner, result, out var ifDirective, orError))
        {
            if (PreprocessorParser.PreCode(ref scanner, result, out var c))
                ifDirective.Code = c;

            var elifDirectives = new List<ElifDirective>();
            while (DirectiveStatementParsers.Elif(ref scanner, result, out var elifDirective, orError))
            {
                elifDirectives.Add((ElifDirective)elifDirective);
                if (PreprocessorParser.PreCode(ref scanner, result, out c))
                    elifDirective.Code = c;
            }

            if (DirectiveStatementParsers.Else(ref scanner, result, out var elseDirective, orError))
                if (PreprocessorParser.PreCode(ref scanner, result, out c))
                    elseDirective.Code = c;

            if (DirectiveStatementParsers.Endif(ref scanner, result, orError))
            {
                parsed = new ConditionalDirectives(ifDirective, scanner.GetLocation(position, scanner.Position - position))
                {
                    Elifs = elifDirectives,
                    Else = elseDirective
                };
                return true;
            }
            else
            {
                parsed = null!;
                return false;
            }
        }
        else
        {
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}

public record struct DirectiveCodeParser : IParser<DirectiveCode>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out DirectiveCode parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        var beginningOfLine = scanner.Position;
        int lineCount = 0;
        while (
            !(
                CommonParsers.Spaces0(ref scanner, result, out _, onlyWhiteSpace: true)
                && (
                    Terminals.Literal("#if", ref scanner)
                    || Terminals.Literal("#define", ref scanner)
                    || Terminals.Literal("#endif", ref scanner)
                    || Terminals.Literal("#elif", ref scanner)
                )
            )
            && !scanner.IsEof
        )
        {
            CommonParsers.Until(ref scanner, '\n', advance: true);
            beginningOfLine = scanner.Position;
            lineCount += 1;
        }
        if (lineCount > 0)
        {
            scanner.Position = beginningOfLine;
            parsed = new DirectiveCode(scanner.GetLocation(position, scanner.Position - position));
            return true;
        }
        else
        {
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}

public record struct ConditionalIfDefDirectivesParser : IParser<IfDefDirective>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out IfDefDirective parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        CommonParsers.Spaces0(ref scanner, result, out _, onlyWhiteSpace: true);
        if (
            Terminals.Literal("#ifdef", ref scanner, advance: true)
            && CommonParsers.Spaces1(ref scanner, result, out _, onlyWhiteSpace: true, orError: new("missing space", new(scanner, scanner.Position)))
            && LiteralsParser.Identifier(ref scanner, result, out var id, new("needs identifier", new(scanner, scanner.Position)))
            && Terminals.EOL(ref scanner, advance: true)
        )
        {
            var cond = new IfDefDirective(id, scanner.GetLocation(position, scanner.Position - position));
            parsed = cond;
            return true;
        }
        else
        {
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}
public record struct ConditionalIfNDefDirectivesParser : IParser<IfNDefDirective>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out IfNDefDirective parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        CommonParsers.Spaces0(ref scanner, result, out _, onlyWhiteSpace: true);
        if (
            Terminals.Literal("#ifndef", ref scanner, advance: true)
            && CommonParsers.Spaces1(ref scanner, result, out _, onlyWhiteSpace: true)
            && LiteralsParser.Identifier(ref scanner, result, out var id)
            && Terminals.EOL(ref scanner, advance: true)
        )
        {
            var cond = new IfNDefDirective(id, scanner.GetLocation(position, scanner.Position - position));
            parsed = cond;
            return true;
        }
        else
        {
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}

public record struct ConditionalIfDirectivesParser : IParser<IfDirective>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out IfDirective parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        CommonParsers.Spaces0(ref scanner, result, out _, onlyWhiteSpace: true);
        if (
            Terminals.Literal("#if", ref scanner, advance: true)
            && CommonParsers.Spaces1(ref scanner, result, out _, onlyWhiteSpace: true)
            && DirectiveExpressionParser.Expression(ref scanner, result, out var expression)
            && Terminals.EOL(ref scanner, advance: true)
        )
        {
            var cond = new IfDirective(expression, scanner.GetLocation(position, scanner.Position - position));
            parsed = cond;
            return true;
        }
        else
        {
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}


public record struct ConditionalElifDirectivesParser : IParser<IfDirective>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out IfDirective parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        CommonParsers.Spaces0(ref scanner, result, out _, onlyWhiteSpace: true);
        if (
            Terminals.Literal("#elif", ref scanner, advance: true)
            && CommonParsers.Spaces1(ref scanner, result, out _, onlyWhiteSpace: true)
            && DirectiveExpressionParser.Expression(ref scanner, result, out var expression)
            && Terminals.EOL(ref scanner, advance: true)
        )
        {
            var cond = new ElifDirective(expression, scanner.GetLocation(position, scanner.Position - position));
            parsed = cond;
            return true;
        }
        else
        {
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}
public record struct ConditionalElseDirectivesParser : IParser<ElseDirective>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out ElseDirective parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        CommonParsers.Spaces0(ref scanner, result, out _, onlyWhiteSpace: true);
        if (
            Terminals.Literal("#else", ref scanner, advance: true)
            && Terminals.EOL(ref scanner, advance: true)
        )
        {
            var cond = new ElseDirective(scanner.GetLocation(position, scanner.Position - position));
            parsed = cond;
            return true;
        }
        else
        {
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}

public record struct EndifDirectiveParser : IParser<NoNode>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out NoNode parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        CommonParsers.Spaces0(ref scanner, result, out _, onlyWhiteSpace: true);

        if (
            Terminals.Literal("#endif", ref scanner, advance: true)
            && Terminals.EOL(ref scanner, advance: true)
        )
        {
            parsed = null!;
            return true;
        }
        else
        {
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}


public record struct ObjectDefineDirective : IParser<AST.ObjectDefineDirective>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out AST.ObjectDefineDirective parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        CommonParsers.Spaces0(ref scanner, result, out _, onlyWhiteSpace: true);

        if (
            Terminals.Literal("#define", ref scanner, advance: true)
            && CommonParsers.Spaces1(ref scanner, result, out _, onlyWhiteSpace: true)
            && LiteralsParser.Identifier(ref scanner, result, out var identifier)
        )
        {
            if (
                DirectiveExpressionParser.Expression(ref scanner, result, out var expression)
                && Terminals.EOL(ref scanner, advance: true)
            )
            {
                parsed = new(identifier, expression, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else if(Terminals.EOL(ref scanner, advance: true))
            {
                parsed = new(identifier, null, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else
            {
                scanner.Position = position;
                parsed = null!;
                return false;
            }
        }
        else
        {
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}


public record struct FunctionDefineDirectiveParser : IParser<FunctionDefineDirective>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out FunctionDefineDirective parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        CommonParsers.Spaces0(ref scanner, result, out _, onlyWhiteSpace: true);

        if (
            Terminals.Literal("#define", ref scanner, advance: true)
            && CommonParsers.Spaces1(ref scanner, result, out _, onlyWhiteSpace: true)
            && LiteralsParser.Identifier(ref scanner, result, out var identifier)
            && CommonParsers.Spaces0(ref scanner, result, out _, onlyWhiteSpace: true)
            && Terminals.Char('(', ref scanner, advance: true)
        )
        {
            var func = new FunctionDefineDirective(identifier, "", new());
            if()
            while(
                CommonParsers.Spaces0(ref scanner, result, out _, onlyWhiteSpace: true)
                && Terminals.Char(',', ref scanner, advance: true)
                && LiteralsParser.Identifier(ref scanner, result, out var paramId)
                && CommonParsers.Spaces0(ref scanner, result, out _, onlyWhiteSpace: true)

            )
            {

            }
        }
        else
        {
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}