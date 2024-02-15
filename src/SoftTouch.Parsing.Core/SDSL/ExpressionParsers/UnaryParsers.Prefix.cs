using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct PrefixParser : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        if (UnaryParsers.PrefixIncrement(ref scanner, result, out parsed))
            return true;
        else if (UnaryParsers.Signed(ref scanner, result, out parsed))
            return true;
        // prefix not
        else if (UnaryParsers.Not(ref scanner, result, out parsed))
            return true;
        // prefix cast 
        else if (UnaryParsers.Cast(ref scanner, result, out parsed))
            return true;
        else if (UnaryParsers.Postfix(ref scanner, result, out var p))
        {
            parsed = p;
            return true;
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            parsed = null!;
            scanner.Position = position;
            return false;
        }
    }
}

public record struct PrefixIncrementParser : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        if (Terminals.Literal("++", ref scanner, advance: true))
        {
            CommonParsers.Spaces0(ref scanner, result, out _);
            if (UnaryParsers.Postfix(ref scanner, result, out var lit))
            {
                parsed = new PrefixExpression(Operator.Inc, lit, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else
            {
                parsed = null!;
                scanner.Position = position;
                result.Errors.Add(new("Expecting Postfix expression", new(scanner, position)));
                return false;
            }
        }
        // prefix decrememnt 
        else if (Terminals.Literal("--", ref scanner, advance: true))
        {
            CommonParsers.Spaces0(ref scanner, result, out _);
            if (UnaryParsers.Postfix(ref scanner, result, out var lit))
            {
                parsed = new PrefixExpression(Operator.Inc, lit, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else
            {
                parsed = null!;
                scanner.Position = position;
                result.Errors.Add(new("Expecting Postfix expression", new(scanner, position)));
                return false;
            }
        }
        else
        {
            if(orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            parsed = null!;
            return false;
        }
    }
}

public record struct NotExpressionParser : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        parsed = null!;
        var position = scanner.Position;
        if (Terminals.Set("!~", ref scanner))
        {
            var op = ((char)scanner.Peek()).ToOperator();
            scanner.Advance(1);
            CommonParsers.Spaces0(ref scanner, result, out _);
            if (UnaryParsers.Postfix(ref scanner, result, out var lit))
            {
                parsed = new PrefixExpression(op, lit, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else
            {
                parsed = null!;
                scanner.Position = position;
                result.Errors.Add(new("Expecting Postfix expression", new(scanner, position)));
                return false;
            }
        }
        else 
        {
            if (orError is not null)
                result.Errors.Add(orError.Value with { Location = new(scanner, position) });
            return false;
        }
    }
}

public record struct SignExpressionParser : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        parsed = null!;
        var position = scanner.Position;
        if (Terminals.Set("+-", ref scanner))
        {
            var op = ((char)scanner.Peek()).ToOperator();
            scanner.Advance(1);
            CommonParsers.Spaces0(ref scanner, result, out _);
            if (UnaryParsers.Prefix(ref scanner, result, out var lit))
            {
                parsed = new PrefixExpression(op, lit, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else
            {
                // TODO: check if error can be added here
                if (orError is not null)
                    result.Errors.Add(orError.Value with { Location = new(scanner, position) });
                parsed = null!;
                scanner.Position = position;
                return false;
            }
        }
        else 
        {
            if (orError is not null)
                result.Errors.Add(orError.Value with { Location = new(scanner, position) });
            return false;
        }
    }
}

public record struct CastExpressionParser : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        if (
                Terminals.Char('(', ref scanner, advance: true)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && LiteralsParser.Identifier(ref scanner, result, out var typeName, new("Expected identifier", new(scanner, scanner.Position)))
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && Terminals.Char(')', ref scanner, true)
                && UnaryParsers.Postfix(ref scanner, result, out var lit)
        )
        {
            parsed = new CastExpression(typeName.Name, Operator.Cast, lit, scanner.GetLocation(position, scanner.Position - position));
            return true;
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value with { Location = new(scanner, position) });
            parsed = null!;
            scanner.Position = position;
            return false;
        }
    }
}