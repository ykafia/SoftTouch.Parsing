using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public struct ExpressionParser : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        if (Ternary(ref scanner, result, out parsed))
            return true;
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            return false;
        }
    }

    public static bool Expression(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        => new ExpressionParser().Match(ref scanner, result, out parsed, in orError);
    public static bool Add(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        => new AdditionParser().Match(ref scanner, result, out parsed, in orError);
    public static bool Mul(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        => new MultiplicationParser().Match(ref scanner, result, out parsed, in orError);
    public static bool Shift(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        => new BitwiseShiftParser().Match(ref scanner, result, out parsed, in orError);
    public static bool Relation(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        => new RelationalParser().Match(ref scanner, result, out parsed, in orError);
    public static bool Equality(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        => new EqualityParser().Match(ref scanner, result, out parsed, in orError);
    public static bool BAnd(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        => new BitwiseAndParser().Match(ref scanner, result, out parsed, in orError);
    public static bool BOr(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        => new BitwiseOrParser().Match(ref scanner, result, out parsed, in orError);
    public static bool XOr(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        => new BitwiseXOrParser().Match(ref scanner, result, out parsed, in orError);
    public static bool And(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        => new AndParser().Match(ref scanner, result, out parsed, in orError);
    public static bool Or(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        => new OrParser().Match(ref scanner, result, out parsed, in orError);
    public static bool Ternary(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
        => new TernaryParser().Match(ref scanner, result, out parsed, in orError);
}


public record struct TernaryParser : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        if (ExpressionParser.Or(ref scanner, result, out parsed))
        {
            var pos2 = scanner.Position;
            CommonParsers.Spaces0(ref scanner, result, out _);
            if (
                Terminals.Char('?', ref scanner, advance: true)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && ExpressionParser.Expression(ref scanner, result, out var left, new("Expected expression", new(scanner, scanner.Position)))
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && Terminals.Char(':', ref scanner, advance: true)
                && CommonParsers.Spaces0(ref scanner, result, out _)
                && ExpressionParser.Expression(ref scanner, result, out var right, new("Expected expression", new(scanner, scanner.Position)))
            )
            {
                parsed = new TernaryExpression(parsed, left, right, scanner.GetLocation(position, scanner.Position - position));
                return true;
            }
            else
            {
                scanner.Position = pos2;
                return true;
            }
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            return false;
        }
    }
}

public record struct OrParser() : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        var ws0 = new Space0();
        parsed = null!;
        ws0.Match(ref scanner, result, out _);
        if (ExpressionParser.And(ref scanner, result, out var left))
        {
            ws0.Match(ref scanner, result, out _);
            if (Terminals.Literal("||", ref scanner))
            {
                var op = scanner.Slice(scanner.Position, 2).ToOperator();
                scanner.Advance(2);
                ws0.Match(ref scanner, result, out _);
                if (ExpressionParser.Or(ref scanner, result, out var shift))
                {
                    parsed = new BinaryExpression(left, op, shift, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else if (ExpressionParser.And(ref scanner, result, out var add))
                {
                    parsed = new BinaryExpression(left, op, add, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else
                {
                    scanner.Position = position;
                    return false;
                }

            }
            else
            {
                parsed = left;
                return true;
            }
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            return false;
        }
    }
}

public record struct AndParser() : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        var ws0 = new Space0();
        parsed = null!;
        ws0.Match(ref scanner, result, out _);
        if (ExpressionParser.BOr(ref scanner, result, out var left))
        {
            ws0.Match(ref scanner, result, out _);
            if (Terminals.Literal("&&", ref scanner))
            {
                var op = scanner.Slice(scanner.Position, 2).ToOperator();
                scanner.Advance(2);
                ws0.Match(ref scanner, result, out _);
                if (ExpressionParser.BAnd(ref scanner, result, out var shift))
                {
                    parsed = new BinaryExpression(left, op, shift, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else if (ExpressionParser.BOr(ref scanner, result, out var add))
                {
                    parsed = new BinaryExpression(left, op, add, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else
                {
                    scanner.Position = position;
                    return false;
                }

            }
            else
            {
                parsed = left;
                return true;
            }
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            return false;
        }
    }
}



public record struct BitwiseOrParser() : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        var ws0 = new Space0();
        parsed = null!;
        ws0.Match(ref scanner, result, out _);
        if (ExpressionParser.XOr(ref scanner, result, out var left))
        {
            ws0.Match(ref scanner, result, out _);
            if (!Terminals.Literal("||", ref scanner) && Terminals.Char('|', ref scanner))
            {
                var op = ((char)scanner.Peek()).ToOperator();
                scanner.Advance(1);
                ws0.Match(ref scanner, result, out _);
                if (ExpressionParser.BOr(ref scanner, result, out var shift))
                {
                    parsed = new BinaryExpression(left, op, shift, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else if (ExpressionParser.XOr(ref scanner, result, out var add))
                {
                    parsed = new BinaryExpression(left, op, add, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else
                {
                    scanner.Position = position;
                    return false;
                }

            }
            else
            {
                parsed = left;
                return true;
            }
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            return false;
        }
    }
}
public record struct BitwiseXOrParser() : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        var ws0 = new Space0();
        parsed = null!;
        ws0.Match(ref scanner, result, out _);
        if (ExpressionParser.BAnd(ref scanner, result, out var left))
        {
            ws0.Match(ref scanner, result, out _);
            if (Terminals.Char('^', ref scanner))
            {
                var op = ((char)scanner.Peek()).ToOperator();
                scanner.Advance(1);
                ws0.Match(ref scanner, result, out _);
                if (ExpressionParser.XOr(ref scanner, result, out var shift))
                {
                    parsed = new BinaryExpression(left, op, shift, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else if (ExpressionParser.BAnd(ref scanner, result, out var add))
                {
                    parsed = new BinaryExpression(left, op, add, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else
                {
                    scanner.Position = position;
                    return false;
                }

            }
            else
            {
                parsed = left;
                return true;
            }
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            return false;
        }
    }
}
public record struct BitwiseAndParser() : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        var ws0 = new Space0();
        parsed = null!;
        ws0.Match(ref scanner, result, out _);
        if (ExpressionParser.Equality(ref scanner, result, out var left))
        {
            ws0.Match(ref scanner, result, out _);
            if (!Terminals.Literal("&&", ref scanner) && Terminals.Char('&', ref scanner))
            {
                var op = ((char)scanner.Peek()).ToOperator();
                scanner.Advance(1);
                ws0.Match(ref scanner, result, out _);
                if (ExpressionParser.BAnd(ref scanner, result, out var shift))
                {
                    parsed = new BinaryExpression(left, op, shift, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else if (ExpressionParser.Equality(ref scanner, result, out var add))
                {
                    parsed = new BinaryExpression(left, op, add, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else
                {
                    scanner.Position = position;
                    return false;
                }

            }
            else
            {
                parsed = left;
                return true;
            }
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            return false;
        }
    }
}



public record struct EqualityParser() : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        var ws0 = new Space0();
        parsed = null!;
        ws0.Match(ref scanner, result, out _);
        if (ExpressionParser.Relation(ref scanner, result, out var left))
        {
            ws0.Match(ref scanner, result, out _);
            if (Terminals.Literal("==", ref scanner) || Terminals.Literal("!=", ref scanner))
            {
                var op = scanner.Slice(scanner.Position, 2).ToOperator();
                scanner.Advance(2);
                ws0.Match(ref scanner, result, out _);
                if (ExpressionParser.Equality(ref scanner, result, out var shift))
                {
                    parsed = new BinaryExpression(left, op, shift, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else if (ExpressionParser.Relation(ref scanner, result, out var add))
                {
                    parsed = new BinaryExpression(left, op, add, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else
                {
                    scanner.Position = position;
                    return false;
                }

            }
            else
            {
                parsed = left;
                return true;
            }
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            return false;
        }
    }
}

public record struct RelationalParser() : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        var ws0 = new Space0();
        parsed = null!;
        ws0.Match(ref scanner, result, out _);
        if (ExpressionParser.Shift(ref scanner, result, out var left))
        {
            ws0.Match(ref scanner, result, out _);
            if (
                !Terminals.Literal(">=", ref scanner) && Terminals.Literal(">", ref scanner)
                || !Terminals.Literal("<=", ref scanner) && Terminals.Literal("<", ref scanner))
            {
                var op = ((char)scanner.Peek()).ToOperator();
                scanner.Advance(1);
                ws0.Match(ref scanner, result, out _);
                if (ExpressionParser.Relation(ref scanner, result, out var shift))
                {
                    parsed = new BinaryExpression(left, op, shift, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else if (ExpressionParser.Shift(ref scanner, result, out var add))
                {
                    parsed = new BinaryExpression(left, op, add, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else
                {
                    scanner.Position = position;
                    return false;
                }

            }
            else if (Terminals.Literal(">=", ref scanner) || Terminals.Literal("<=", ref scanner))
            {
                var op = scanner.Slice(scanner.Position, 2).ToOperator();
                scanner.Advance(2);
                ws0.Match(ref scanner, result, out _);
                if (ExpressionParser.Relation(ref scanner, result, out var shift))
                {
                    parsed = new BinaryExpression(left, op, shift, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else if (ExpressionParser.Shift(ref scanner, result, out var add))
                {
                    parsed = new BinaryExpression(left, op, add, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else
                {
                    scanner.Position = position;
                    return false;
                }

            }
            else
            {
                parsed = left;
                return true;
            }
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            return false;
        }
    }
}

public record struct BitwiseShiftParser() : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        var ws0 = new Space0();
        parsed = null!;
        ws0.Match(ref scanner, result, out _);
        if (ExpressionParser.Add(ref scanner, result, out var left))
        {
            ws0.Match(ref scanner, result, out _);
            if (Terminals.Literal(">>", ref scanner) || Terminals.Literal("<<", ref scanner))
            {
                var op = scanner.Slice(scanner.Position, 2).ToOperator();
                scanner.Advance(2);
                ws0.Match(ref scanner, result, out _);
                if (ExpressionParser.Shift(ref scanner, result, out var shift))
                {
                    parsed = new BinaryExpression(left, op, shift, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else if (ExpressionParser.Add(ref scanner, result, out var add))
                {
                    parsed = new BinaryExpression(left, op, add, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else
                {
                    scanner.Position = position;
                    return false;
                }

            }
            else
            {
                parsed = left;
                return true;
            }
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            return false;
        }
    }
}

public record struct AdditionParser() : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        var ws0 = new Space0();
        parsed = null!;
        ws0.Match(ref scanner, result, out _);
        if (ExpressionParser.Mul(ref scanner, result, out var left))
        {
            ws0.Match(ref scanner, result, out _);
            if (Terminals.Set("+-", ref scanner))
            {
                var op = ((char)scanner.Peek()).ToOperator();
                scanner.Advance(1);
                ws0.Match(ref scanner, result, out _);
                if (ExpressionParser.Add(ref scanner, result, out var add))
                {
                    parsed = new BinaryExpression(left, op, add, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else if (ExpressionParser.Mul(ref scanner, result, out var mul))
                {
                    parsed = new BinaryExpression(left, op, mul, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else
                {
                    scanner.Position = position;
                    return false;
                }

            }
            else
            {
                parsed = left;
                return true;
            }
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            return false;
        }
    }
}

public record struct MultiplicationParser() : IParser<Expression>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out Expression parsed, in ParseError? orError = null)
    {
        var position = scanner.Position;
        var ws0 = new Space0();
        parsed = null!;
        ws0.Match(ref scanner, result, out _);
        if (UnaryParsers.Prefix(ref scanner, result, out var left))
        {
            ws0.Match(ref scanner, result, out _);
            if (Terminals.Set("*/%", ref scanner))
            {
                var op = ((char)scanner.Peek()).ToOperator();
                scanner.Advance(1);
                ws0.Match(ref scanner, result, out _);
                if (ExpressionParser.Mul(ref scanner, result, out var expression))
                {
                    parsed = new BinaryExpression(left, op, expression, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                else if (UnaryParsers.Prefix(ref scanner, result, out var right))
                {
                    parsed = new BinaryExpression(left, op, right, scanner.GetLocation(position, scanner.Position - position));
                    return true;
                }
                scanner.Position = position;
                return false;
            }
            else
            {
                parsed = left;
                return true;
            }
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            return false;
        }
    }
}