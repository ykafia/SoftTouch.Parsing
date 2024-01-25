using System.Drawing;
using System.Numerics;

namespace SoftTouch.Parsing.SDSL.AST;


public abstract class Literal(TextLocation info) : Node(info);


public abstract class NumberLiteral<T>(int size, T value, TextLocation info) : Literal(info)
    where T : struct, INumber<T>
{
    public int Size { get; set; } = size;
    public T Value { get; set; } = value;

    public override string ToString()
    {
        return $"[Number : {Value}]";
    }
}

public class IntegerLiteral(int size, long value, TextLocation info) : NumberLiteral<long>(size, value, info);

public sealed class FloatLiteral(int size, double value, TextLocation info) : NumberLiteral<double>(size, value, info);

public sealed class HexLiteral(int size, long value, TextLocation info) : IntegerLiteral(size, value, info);


public class BoolLiteral(bool value, TextLocation info) : Literal(info)
{
    public bool Value { get; set; } = value;
}


public class Identifier(string name, TextLocation info) : Literal(info)
{
    public string Name { get; set; } = name;
}
