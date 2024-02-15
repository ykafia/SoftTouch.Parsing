using System.Text;

namespace SoftTouch.Parsing.SDSL.AST;

public abstract class Statement(TextLocation info) : ValueNode(info);

public class ExpressionStatement(Expression expression, TextLocation info) : Statement(info)
{
    public Expression Expression { get; set; } = expression;
    public override string ToString()
    {
        return $"{Expression};";
    }
}

public class Declare(Identifier typename, Identifier name, TextLocation info) : Statement(info)
{
    public Identifier TypeName { get; set; } = typename;
    public Identifier VariableName { get; set; } = name;

    public override string ToString()
    {
        return $"{TypeName} {VariableName};";
    }
}


public class DeclareAssign(Identifier typename, Identifier name, Expression value, TextLocation info) : Declare(typename, name, info)
{
    public Expression Value { get; set; } = value;
    public override string ToString()
    {
        return $"{TypeName} {VariableName} = {Value};";
    }
}

public class Assign(Expression assigned, AssignOperator op, Expression value, TextLocation info) : Statement(info)
{
    public Expression Assigned { get; set; } = assigned;
    public AssignOperator Operator { get; set; } = op;
    public Expression Value { get; set; } = value;

    public override string ToString()
    {
        return $"{Assigned} {Operator.ToAssignSymbol()} {Value};";
    }
}



public class BlockStatement(TextLocation info) : Statement(info)
{
    public List<Statement> Statements { get; set; } = [];

    public override string ToString()
    {
        var builder = new StringBuilder().Append("Block : \n");
        foreach (var e in Statements)
            builder.AppendLine(e.ToString());
        return builder.AppendLine("End").ToString();
    }
}
