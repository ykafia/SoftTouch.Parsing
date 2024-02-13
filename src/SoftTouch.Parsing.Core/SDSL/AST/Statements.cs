namespace SoftTouch.Parsing.SDSL.AST;


public abstract class Statement(TextLocation info) : ValueNode(info);


public class BlockStatement(TextLocation info) : Statement(info)
{
    public List<Statement> Statements { get; set; } = [];
}

public class Declare(Identifier typename, Identifier name, TextLocation info) : Statement(info)
{
    public Identifier TypeName { get; set; } = typename;
    public Identifier VariableName { get; set; } = name;
}


public class DeclareAssign(Identifier typename, Identifier name, Expression value, TextLocation info) : Declare(typename, name, info)
{
    public Expression Value { get; set; } = value;
}

public class Assign(Expression assigned, AssignOperator op, Expression value, TextLocation info) : Statement(info)
{
    public Expression Assigned { get; set; } = assigned;
    public AssignOperator Operator { get; set; } = op;
    public Expression Value { get; set; } = value;
}

public abstract class Flow(TextLocation info) : Statement(info);

public class ForEach(Identifier typename, Identifier variable, Expression collection, TextLocation info) : Flow(info)
{
    public Identifier Typename { get; set; } = typename;
    public Identifier Variable { get; set; } = variable;
    public Expression Collection { get; set; } = collection;
}

public class While(Expression condition, Expression collection, TextLocation info) : Flow(info)
{
    public Expression Condition { get; set; } = condition;
    public Expression Collection { get; set; } = collection;
}