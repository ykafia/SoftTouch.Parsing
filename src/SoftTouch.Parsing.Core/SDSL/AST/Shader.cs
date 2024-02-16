namespace SoftTouch.Parsing.SDSL.AST;


public class ShaderClass(Identifier name, TextLocation info) : Node(info)
{
    public Identifier Name { get; set; } = name;
    public List<ShaderElement> Elements { get; set; } = [];
    public List<ShaderGenerics> Generics { get; set; } = [];
    public List<ShaderMixin> Mixins { get; set; } = [];
}


public class ShaderGenerics(Identifier typename, Identifier name, TextLocation info) : Node(info)
{
    public Identifier Name { get; set; } = name;
    public Identifier TypeName { get; set; } = typename;
}

public class ShaderMixin(TextLocation info) : Node(info)
{
    public List<ShaderMixinValue> Generics { get; set; } = [];
}

public abstract class ShaderMixinValue(TextLocation info) : Node(info);
public class ShaderMixinExpression(Expression expression, TextLocation info) : ShaderMixinValue(info)
{
    public Expression Value { get; set; } = expression;
}
public class ShaderMixinIdentifier(Identifier identifier, TextLocation info) : ShaderMixinValue(info)
{
    public Identifier Value { get; set; } = identifier;
}