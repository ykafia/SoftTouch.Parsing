namespace SoftTouch.Parsing.Core;


public abstract class Parser
{
    public abstract bool ParseInternal(ref StringScanner scanner);
}