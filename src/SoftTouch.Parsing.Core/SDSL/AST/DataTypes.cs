namespace SoftTouch.Parsing.SDSL.AST;

public abstract record DataType();
public record Scalar() : DataType();
public record Vector(int Size) : DataType();
public record Matrix(int Rows, int Columns) : DataType();
public record Array(int Size) : DataType();
public record Struct(Dictionary<string, DataType> Fields) : DataType();