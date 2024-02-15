namespace SoftTouch.Parsing.SDSL.AST;


public abstract class ShaderElement(TextLocation info) : Node(info);


public class ShaderMethod(Identifier name, TextLocation info, Identifier? visibility = null, Identifier? storage = null, bool isStaged = false, bool isAbstract = false, bool isVirtual = false, bool isOverride = false, bool isClone = false) : ShaderElement(info)
{
    public Identifier Name { get; set; } = name;
    public Identifier? Visibility { get; set; } = visibility;
    public Identifier? Storage { get; set; } = storage;
    public bool? IsStaged { get; set; } = isStaged;
    public bool? IsAbstract { get; set; } = isAbstract;
    public bool? IsVirtual { get; set; } = isVirtual;
    public bool? IsOverride { get; set; } = isOverride;
    public bool? IsClone { get; set; } = isClone;
    public BlockStatement? Body { get; set; }
}