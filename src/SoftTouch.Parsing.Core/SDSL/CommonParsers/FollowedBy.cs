using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public record struct FollowedBy<TParser, TData>
    where TParser : struct, IParser<TData>
    where TData : Node
{
    public readonly bool Match(ref Scanner scanner, bool resetPosition = true)
    {
        var position = scanner.Position;
        var p = new TParser();
        if(p.Match(ref scanner, null, out _))
        {
            if(resetPosition)
                scanner.Position = position;
            return true;
        }
        return false;
    }
}

public record struct FollowedByTerminal<TParser>
    where TParser : struct, ITerminal
{
    public readonly bool Match(ref Scanner scanner, bool advance = false)
    {
        var position = scanner.Position;
        var p = new TParser();
        return p.Match(ref scanner, advance);
    }
}