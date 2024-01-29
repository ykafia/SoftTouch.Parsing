namespace SoftTouch.Parsing.SDSL.AST;

public enum Operator
{
    Nop,
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
    Add,
    Sub,
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
    public static Operator ToOperator(this string s)
    {
        return s switch
        {
            "!" => Operator.Not,
            "~" => Operator.BitwiseNot,
            "++" => Operator.Inc,
            "--" => Operator.Dec,
            "+" => Operator.Add,
            "-" => Operator.Sub,
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
}




public class Expression(ValueNode left, Operator op, ValueNode right, TextLocation info) : Node(info)
{
    public Operator Op { get; set; } = op;
    public ValueNode Left { get; set; } = left;
    public ValueNode Right { get; set; } = right;
}