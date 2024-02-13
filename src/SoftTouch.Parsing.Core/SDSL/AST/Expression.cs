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
    LogicalOR,
    Accessor,
    Indexer
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
    public static string ToSymbol(this Operator s)
    {
        return s switch
        {
            Operator.Not => "!",
            Operator.BitwiseNot => "~",
            Operator.Inc => "++",
            Operator.Dec => "--",
            Operator.Plus => "+",
            Operator.Minus => "-",
            Operator.Mul => "*",
            Operator.Div => "/",
            Operator.Mod => "%",
            Operator.RightShift => ">>",
            Operator.LeftShift => "<<",
            Operator.AND => "&",
            Operator.OR => "|",
            Operator.XOR => "^",
            Operator.Greater => ">",
            Operator.Lower => "<",
            Operator.GreaterOrEqual => ">=",
            Operator.LowerOrEqual => "<=",
            Operator.Equals => "==",
            Operator.NotEquals => "!=",
            Operator.LogicalAND => "&&",
            Operator.LogicalOR => "||",
            _ => "NOp"
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

public class MethodCall(Identifier name, TextLocation info) : Expression(info)
{
    public Identifier Name = name;
    public List<Expression> Parameters = [];

    public override string ToString()
    {
        return $"{Name}({string.Join(", ", Parameters)})";
    }
}


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

public class PostfixExpression(Expression expression, Operator op, TextLocation info) : UnaryExpression(expression, op, info)
{
    public override string ToString()
    {
        return $"{Expression}{Operator.ToSymbol()}";
    }
}

public class AccessorExpression(Expression expression, Expression accessed, TextLocation info) : PostfixExpression(expression, Operator.Accessor, info)
{
    public Expression Accessed { get; set; } = accessed;

    public override string ToString()
    {
        return $"{Expression}.{Accessed}";
    }
}

public class IndexerExpression(Expression expression, Expression index, TextLocation info) : PostfixExpression(expression, Operator.Indexer, info)
{
    public Expression Index { get; set; } = index;
    public override string ToString()
    {
        return $"{Expression}[{Index}]";
    }
}


public class BinaryExpression(Expression left, Operator op, Expression right, TextLocation info) : Expression(info)
{
    public Operator Op { get; set; } = op;
    public Expression Left { get; set; } = left;
    public Expression Right { get; set; } = right;

    public override string ToString()
    {
        return $"( {Left} {Op.ToSymbol()} {Right} )";
    }
}

public class TernaryExpression(Expression cond, Expression left, Expression right, TextLocation info) : Expression(info)
{
    public Expression Condition { get; set; } = cond;
    public Expression Left { get; set; } = left;
    public Expression Right { get; set; } = right;

    public override string ToString()
    {
        return $"({Condition} ? {Left} : {Right})";
    }
}




public class ValueExpression(Literal value) : Expression(value.Info)
{
    public Literal Value { get; set; } = value;
    public override string ToString() => Value.ToString() ?? "";
}