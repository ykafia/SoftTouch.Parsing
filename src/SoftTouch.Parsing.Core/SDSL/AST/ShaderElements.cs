namespace SoftTouch.Parsing.SDSL.AST;


public abstract class ShaderElement(TextLocation info) : Node(info);

public abstract class ShaderBuffer(Identifier name, TextLocation info) : ShaderElement(info)
{
    public Identifier Name { get; set; } = name;
}

public sealed class CBuffer(Identifier name, TextLocation info) : ShaderBuffer(name, info)
{
    public List<ShaderMember> Members { get; set; } = [];
}
public sealed class TBuffer(Identifier name, TextLocation info) : ShaderBuffer(name, info)
{
    public List<ShaderMember> Members { get; set; } = [];
}

public abstract class MethodOrMember(TextLocation info, bool isStaged = false) : ShaderElement(info)
{
    public bool IsStaged { get; set; } = isStaged;
}


public sealed class ShaderMember(Identifier type, Identifier name, Expression? initialValue, TextLocation location, bool isStaged = false, bool isStream = false, Identifier? semantic = null) : MethodOrMember(location, isStaged)
{
    public Identifier Type { get; set; } = type;
    public Identifier Name { get; set; } = name;
    public Identifier? Semantic { get; set; } = semantic;
    public bool IsStream { get; set;} = isStream;
    public Expression? Value { get; set; } = initialValue;

    public override string ToString()
    {
        return $"{Type} {Name}";
    }
}

public class ShaderMethod(Identifier name, Identifier returnType, TextLocation info, Identifier? visibility = null, Identifier? storage = null, bool isStaged = false, bool isAbstract = false, bool isVirtual = false, bool isOverride = false, bool isClone = false) : MethodOrMember(info, isStaged)
{
    public Identifier Name { get; set; } = name;
    public Identifier ReturnType { get; set; } = returnType;
    public Identifier? Visibility { get; set; } = visibility;
    public Identifier? Storage { get; set; } = storage;
    public bool? IsAbstract { get; set; } = isAbstract;
    public bool? IsVirtual { get; set; } = isVirtual;
    public bool? IsOverride { get; set; } = isOverride;
    public bool? IsClone { get; set; } = isClone;
    public BlockStatement? Body { get; set; }

    public override string ToString()
    {
        return $"{ReturnType} {Name}()\n{Body}\n";
    }
}