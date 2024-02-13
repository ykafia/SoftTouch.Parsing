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

public abstract class Loop(TextLocation info) : Flow(info);

public class ForEach(Identifier typename, Identifier variable, Expression collection, TextLocation info) : Loop(info)
{
    public Identifier Typename { get; set; } = typename;
    public Identifier Variable { get; set; } = variable;
    public Expression Collection { get; set; } = collection;
    public List<Statement> Body { get; set; } = [];
}


public abstract class ForInitializer(TextLocation info) : Node(info);
public abstract class InitializerDeclare(Identifier typename, Identifier name, Expression value, TextLocation info) : Node(info)
{
    public Identifier TypeName { get; set; } = typename;
    public Identifier VariableName { get; set; } = name;
    public Expression Value { get; set; } = value;
}

public class For(ForInitializer initializer, Expression cond, Statement update, TextLocation info) : Loop(info)
{
    public ForInitializer Initializer { get; set; } = initializer;
    public Expression Condition { get; set; } = cond;
    public Statement Update { get; set; } = update;
    public List<Statement> Body { get; set; } = [];
}

public class While(Expression condition, TextLocation info) : Loop(info)
{
    public Expression Condition { get; set; } = condition;
    public List<Statement> Body { get; set; } = [];
}