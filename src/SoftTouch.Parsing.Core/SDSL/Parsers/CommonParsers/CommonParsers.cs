using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public static class CommonParsers
{
    public static bool Spaces0<TScanner>(ref TScanner scanner, ParseResult result, out NoNode node, in ParseError? orError = null, bool onlyWhiteSpace = false)
        where TScanner : struct, IScanner
        => new Space0(onlyWhiteSpace).Match(ref scanner, result, out node, in orError);
    public static bool Spaces1<TScanner>(ref TScanner scanner, ParseResult result, out NoNode node, in ParseError? orError = null, bool onlyWhiteSpace = false)
       where TScanner : struct, IScanner
        => new Space1(onlyWhiteSpace).Match(ref scanner, result, out node, in orError);


    public static bool Optional<TScanner, TTerminal>(ref TScanner scanner, TTerminal terminal, bool advance = false)
        where TScanner : struct, IScanner
        where TTerminal : struct, ITerminal
    {
        terminal.Match(ref scanner, advance: advance);
        return true;
    }
    public static bool Optional<TScanner, TNode>(ref TScanner scanner, IParser<TNode> parser, ParseResult result, out TNode? node)
        where TScanner : struct, IScanner
        where TNode : Node
    {
        parser.Match(ref scanner, result, out node);
        return true;
    }

    public static bool FollowedBy<TScanner, TTerminal>(ref TScanner scanner, TTerminal terminal, bool withSpaces = false)
        where TScanner : struct, IScanner
        where TTerminal : struct, ITerminal
    {
        var position = scanner.Position;
        if (withSpaces)
            Spaces0(ref scanner, null!, out _);
        if (terminal.Match(ref scanner, advance: false))
        {
            scanner.Position = position;
            return true;
        }
        scanner.Position = position;
        return false;
    }

    public static bool Until<TScanner>(ref TScanner scanner, char value, bool advance = false)
        where TScanner : struct, IScanner
    {
        while (!scanner.IsEof && !Terminals.Char(value, ref scanner, advance))
            scanner.Advance(1);
        return scanner.IsEof;
    }
    public static bool Until<TScanner>(ref TScanner scanner, string value, bool advance = false)
        where TScanner : struct, IScanner
    {
        while (!scanner.IsEof && !Terminals.Literal(value, ref scanner, advance))
            scanner.Advance(1);
        return scanner.IsEof;
    }
    public static bool Until<TScanner>(ref TScanner scanner, ReadOnlySpan<string> values, bool advance = false)
        where TScanner : struct, IScanner
    {
        while (!scanner.IsEof)
        {
            foreach (var value in values)
                if (Terminals.Literal(value, ref scanner, advance))
                    return scanner.IsEof;
            scanner.Advance(1);
        }
        return scanner.IsEof;
    }
    public static bool Until<TScanner, TTerminal>(ref Scanner scanner, bool advance = false)
        where TScanner : struct, IScanner
        where TTerminal : struct, ITerminal
    {
        var t = new TTerminal();
        while (!scanner.IsEof && !t.Match(ref scanner, advance))
            scanner.Advance(1);
        return !scanner.IsEof;
    }
    public static bool Until<TScanner, TTerminal1, TTerminal2>(ref Scanner scanner, TTerminal1? terminal1 = null, TTerminal2? terminal2 = null, bool advance = false)
        where TScanner : struct, IScanner
        where TTerminal1 : struct, ITerminal
        where TTerminal2 : struct, ITerminal
    {
        var t1 = terminal1 ?? new TTerminal1();
        var t2 = terminal2 ?? new TTerminal2();
        while (!scanner.IsEof && !(t1.Match(ref scanner, advance) || t2.Match(ref scanner, advance)))
            scanner.Advance(1);
        return !scanner.IsEof;
    }
    public static bool Until<TScanner, TTerminal1, TTerminal2, TTerminal3>(ref Scanner scanner, TTerminal1? terminal1 = null, TTerminal2? terminal2 = null, TTerminal3? terminal3 = null, bool advance = false)
        where TScanner : struct, IScanner
        where TTerminal1 : struct, ITerminal
        where TTerminal2 : struct, ITerminal
        where TTerminal3 : struct, ITerminal
    {
        var t1 = terminal1 ?? new TTerminal1();
        var t2 = terminal2 ?? new TTerminal2();
        var t3 = terminal3 ?? new TTerminal3();
        while (!scanner.IsEof && !(t1.Match(ref scanner, advance) || t2.Match(ref scanner, advance) || t3.Match(ref scanner, advance)))
            scanner.Advance(1);
        return !scanner.IsEof;
    }
}