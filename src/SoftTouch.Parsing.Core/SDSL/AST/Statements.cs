using System.Text;

namespace SoftTouch.Parsing.SDSL.AST;

public abstract class Statement(TextLocation info) : ValueNode(info);

public class EmptyStatement(TextLocation info) : Statement(info)
{
    public override string ToString() => ";";
}

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

public class Declare(TypeName typename, TextLocation info) : Declaration(typename, info)
{
    public List<VariableAssign> Variables { get; set; } = [];

    public override string ToString()
    {
        return $"{TypeName} {string.Join(", ", Variables.Select(v => v.ToString()))}";
    }
}

public class Assign(TextLocation info) : Statement(info)
{
    public List<VariableAssign> Variables { get; set; } = [];
    public override string ToString()
    {
        return string.Join(", ", Variables.Select(x => x.ToString())) + ";";
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
