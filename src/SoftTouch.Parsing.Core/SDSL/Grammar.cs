using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public static class Grammar
{
    public static Node Match(string code)
    {
        var p = new IntegerParser();
        var scanner = new Scanner(code);
        p.Match(ref scanner, out var num);
        return num;
    }
}