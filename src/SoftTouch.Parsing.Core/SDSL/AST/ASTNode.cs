namespace SoftTouch.Parsing.SDSL.AST;

public abstract class Node(TextLocation info)
{
    public TextLocation Info { get; set; } = info;
}
public class ValueNode(TextLocation info) : Node(info)
{
    public string? Type { get; set; } = null;
}
public class NoNode() : Node(new());
