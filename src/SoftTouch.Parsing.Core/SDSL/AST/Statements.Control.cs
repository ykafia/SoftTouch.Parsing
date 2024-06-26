namespace SoftTouch.Parsing.SDSL.AST;


public abstract class Control(TextLocation info) : Flow(info);


public class ConditionalFlow(If first, TextLocation info) : Flow(info)
{
    public If If { get; set; } = first;
    public List<ElseIf> ElseIfs { get; set; } = [];
    public Else? Else { get; set; }
}
public class If(Expression condition, Statement body, TextLocation info) : Flow(info)
{
    public Expression Condition { get; set; } = condition;
    public Statement Body { get; set; } = body;
}

public class ElseIf(Expression condition, Statement body, TextLocation info) : If(condition, body, info);

public class Else(Statement body, TextLocation info) : Flow(info)
{
    public Statement Body { get; set; } = body;
}