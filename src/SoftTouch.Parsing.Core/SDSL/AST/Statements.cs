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

public class Return(TextLocation info, Expression? expression = null) : Statement(info)
{
    public Expression? Value { get; set; } = expression;

    public override string ToString()
    {
        return $"return {Value};";
    }
}

public abstract class Declaration(TypeName typename, TextLocation info) : Statement(info)
{
    public TypeName TypeName { get; set; } = typename;
}

public class Declare(TypeName typename, Identifier name, TextLocation info) : Declaration(typename, info)
{
    public Identifier Name { get; set; } = name;
    public Expression? Value { get; set; }

    public override string ToString()
    {
        return $"{TypeName} {Name};";
    }
}

public class VariableAssign(Identifier name, TextLocation info, AssignOperator? op = null,  Expression? value = null) : Node(info)
{
    public Identifier Name { get; set; } = name;
    public AssignOperator? Operator { get; set; } = op;
    public Expression? Value { get; set; } = value;

    public override string ToString()
        => Value switch
        {
            null => Name.Name,
            Expression v => $"{Name} = {v}"
        };
}

public class MultiAssign(TypeName typename, TextLocation info) : Declaration(typename, info)
{
    public List<VariableAssign> Variables { get; set; } = [];
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
