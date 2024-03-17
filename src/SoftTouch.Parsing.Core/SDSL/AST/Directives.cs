namespace SoftTouch.Parsing.SDSL.AST;


public abstract class Directive(Expression expression, TextLocation info) : Node(info)
{
    public Expression Expression { get; set; } = expression;
}


public class IfDirective(Expression expression, TextLocation info) : Directive(expression, info);
public class ElifDirective(Expression expression, TextLocation info) : Directive(expression, info);
public class ElseDirective(TextLocation info) : Directive(null!, info);

public class ConditionalDirectives(IfDirective ifExp, TextLocation info) : Node(info)
{
    public IfDirective If { get; set; } = ifExp;
    public List<ElifDirective> Elifs { get; set; } = [];
    public ElseDirective? Else { get; set; }
}
