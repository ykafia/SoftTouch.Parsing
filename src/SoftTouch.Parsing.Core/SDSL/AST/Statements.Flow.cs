namespace SoftTouch.Parsing.SDSL.AST;

public abstract class Flow(TextLocation info) : Statement(info);

public abstract class Loop(TextLocation info) : Flow(info);
public class Break(TextLocation info) : Statement(info);
public class Continue(TextLocation info) : Statement(info);


public class ForEach(TypeName typename, Identifier variable, Expression collection, Statement body, TextLocation info) : Loop(info)
{
    public TypeName Typename { get; set; } = typename;
    public Identifier Variable { get; set; } = variable;
    public Expression Collection { get; set; } = collection;
    public Statement Body { get; set; } = body;
}


public abstract class ForInitializer(TextLocation info) : Node(info);
public abstract class InitializerDeclare(TypeName typename, Identifier name, Expression value, TextLocation info) : Node(info)
{
    public TypeName TypeName { get; set; } = typename;
    public Identifier VariableName { get; set; } = name;
    public Expression Value { get; set; } = value;
}


public class While(Expression condition, Statement body, TextLocation info) : Loop(info)
{
    public Expression Condition { get; set; } = condition;
    public Statement Body { get; set; } = body;
}

public enum ForAnnotationKind
{
    Unroll,
    Loop,
    Fastopt,
    AllowUAVCondition
}
public record struct ForAnnotation(ForAnnotationKind Kind, int? Count = null);

public class For(ForInitializer initializer, Expression cond, Statement update, TextLocation info) : Loop(info)
{
    public ForInitializer Initializer { get; set; } = initializer;
    public Expression Condition { get; set; } = cond;
    public Statement Update { get; set; } = update;
    public List<Statement> Body { get; set; } = [];
    public List<ForAnnotation> Annotations { get; set; } = [];
}

