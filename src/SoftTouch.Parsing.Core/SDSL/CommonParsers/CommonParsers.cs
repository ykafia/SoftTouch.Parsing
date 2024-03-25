using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public static class CommonParsers
{
    public static bool Spaces0(ref Scanner scanner, ParseResult result, out NoNode node, in ParseError? orError = null, bool onlyWhiteSpace = false)
        => new Space0(onlyWhiteSpace).Match(ref scanner, result, out node, in orError);
    public static bool Spaces1(ref Scanner scanner, ParseResult result, out NoNode node, in ParseError? orError = null, bool onlyWhiteSpace = false)
        => new Space1(onlyWhiteSpace).Match(ref scanner, result, out node, in orError);

    public static bool Until(ref Scanner scanner, char value, bool advance = false)
    {
        while(!scanner.IsEof && !Terminals.Char(value, ref scanner, advance))
            scanner.Advance(1);
        return scanner.IsEof;
    }
    public static bool Until(ref Scanner scanner, string value, bool advance = false)
    {
        while (!scanner.IsEof && !Terminals.Literal(value, ref scanner, advance))
            scanner.Advance(1);
        return scanner.IsEof;
    }
    public static bool Until<TTerminal>(ref Scanner scanner, bool advance = false)
        where TTerminal : struct, ITerminal
    {
        var t = new TTerminal();
        while (!scanner.IsEof && !t.Match(ref scanner, advance))
            scanner.Advance(1);
        return !scanner.IsEof;
    }
    public static bool Until<TTerminal1, TTerminal2>(ref Scanner scanner, TTerminal1? terminal1 = null, TTerminal2? terminal2 = null, bool advance = false)
        where TTerminal1 : struct, ITerminal
        where TTerminal2 : struct, ITerminal
    {
        var t1 = terminal1 ?? new TTerminal1();
        var t2 = terminal2 ?? new TTerminal2();
        while (!scanner.IsEof && !(t1.Match(ref scanner, advance) || t2.Match(ref scanner, advance)))
            scanner.Advance(1);
        return !scanner.IsEof;
    }
    public static bool Until<TTerminal1, TTerminal2, TTerminal3>(ref Scanner scanner, TTerminal1? terminal1 = null, TTerminal2? terminal2 = null, TTerminal3? terminal3 = null, bool advance = false)
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