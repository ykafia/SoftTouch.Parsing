namespace SoftTouch.Parsing.SDSL.AST;


public abstract class Control(TextLocation info) : Flow(info);


public class ConditionalFlow(If first, TextLocation info) : Flow(info)
{
    public If If { get; set; } = first;
    public List<ElseIf> ElseIfs { get; set; } = [];
    public Else? Else { get; set; }
}
public class If(Expression condition, TextLocation info) : Flow(info)
{
    public Expression Condition { get; set; } = condition;
    public List<Statement> Body { get; set; } = [];
}

public class ElseIf(Expression condition, TextLocation info) : If(condition, info);


public class Else(TextLocation info) : Flow(info)
{
    public List<Statement> Body { get; set; } = [];
}