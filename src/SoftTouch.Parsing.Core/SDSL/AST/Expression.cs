namespace SoftTouch.Parsing.SDSL.AST;

public enum Operator
{
    Nop,
    Cast,
    Positive,
    Negative,
    Not,
    /// <summary>
    /// Bitwise not
    /// </summary>
    BitwiseNot,
    /// <summary>
    /// Increment
    /// </summary>
    Inc,
    /// <summary>
    /// Decrement
    /// </summary>
    Dec,
    Plus,
    Minus,
    Mul,
    Div,
    Mod,
    RightShift,
    LeftShift,
    AND,
    OR,
    XOR,
    Greater,
    Lower,
    GreaterOrEqual,
    LowerOrEqual,
    NotEquals,
    Equals,
    LogicalAND,
    LogicalOR
}

public static class StringOperatorExtensions
{
    public static Operator ToOperator(this ReadOnlySpan<char> s)
    {
        return s switch
        {
            "!" => Operator.Not,
            "~" => Operator.BitwiseNot,
            "++" => Operator.Inc,
            "--" => Operator.Dec,
            "+" => Operator.Plus,
            "-" => Operator.Minus,
            "*" => Operator.Mul,
            "/" => Operator.Div,
            "%" => Operator.Mod,
            ">>" => Operator.RightShift,
            "<<" => Operator.LeftShift,
            "&" => Operator.AND,
            "|" => Operator.OR,
            "^" => Operator.XOR,
            ">" => Operator.XOR,
            "<" => Operator.XOR,
            ">=" => Operator.XOR,
            "<=" => Operator.XOR,
            "==" => Operator.Equals,
            "!=" => Operator.NotEquals,
            "&&" => Operator.LogicalAND,
            "||" => Operator.LogicalOR,
            _ => Operator.Nop,
        };
    }
    public static Operator ToOperator(this string s)
    {
        return s switch
        {
            "!" => Operator.Not,
            "~" => Operator.BitwiseNot,
            "++" => Operator.Inc,
            "--" => Operator.Dec,
            "+" => Operator.Plus,
            "-" => Operator.Minus,
            "*" => Operator.Mul,
            "/" => Operator.Div,
            "%" => Operator.Mod,
            ">>" => Operator.RightShift,
            "<<" => Operator.LeftShift,
            "&" => Operator.AND,
            "|" => Operator.OR,
            "^" => Operator.XOR,
            ">" => Operator.Greater,
            "<" => Operator.Lower,
            ">=" => Operator.GreaterOrEqual,
            "<=" => Operator.LowerOrEqual,
            "==" => Operator.Equals,
            "!=" => Operator.NotEquals,
            "&&" => Operator.LogicalAND,
            "||" => Operator.LogicalOR,
            _ => Operator.Nop,
        };
    }

    public static Operator ToOperator(this char c)
    {
        return c switch
        {
            '!' => Operator.Not,
            '~' => Operator.BitwiseNot,
            '+' => Operator.Plus,
            '-' => Operator.Minus,
            '*' => Operator.Mul,
            '/' => Operator.Div,
            '%' => Operator.Mod,
            '&' => Operator.AND,
            '|' => Operator.OR,
            '^' => Operator.XOR,
            '>' => Operator.Greater,
            '<' => Operator.Lower,
            _ => Operator.Nop,
        };
    }
}


public abstract class Expression(TextLocation info) : ValueNode(info);


public abstract class UnaryExpression(Expression expression, Operator op, TextLocation info) : Expression(info)
{
    public Expression Expression { get; set; } = expression;
    public Operator Operator { get; set; } = op;
}

public class PrefixExpression(Operator op, Expression expression, TextLocation info) : UnaryExpression(expression, op, info);

public class CastExpression(string typeName, Operator op, Expression expression, TextLocation info) : PrefixExpression(op, expression, info)
{
    public string TypeName { get; set; } = typeName;
}

public class PostfixExpression(Expression expression, Operator op, TextLocation info) : UnaryExpression(expression, op, info);


public class BinaryExpression(Expression left, Operator op, Expression right, TextLocation info) : Expression(info)
{
    public Operator Op { get; set; } = op;
    public Expression Left { get; set; } = left;
    public Expression Right { get; set; } = right;

    public override string ToString()
    {
        return $"({Left} {Op} {Right})";
    }
}



public class ValueExpression(Literal value) : Expression(value.Info)
{
    public Literal Value { get; set; } = value;
    public override string ToString() => Value.ToString() ?? "";
}