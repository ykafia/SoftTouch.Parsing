using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public struct NumberParser : IParser<NumberLiteral>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out NumberLiteral parsed)
    {
        var fp = new FloatParser();
        var ip = new IntegerParser();

        if (fp.Match(ref scanner, result, out FloatLiteral pf))
        {
            parsed = pf;
            return true;
        }
        else if (ip.Match(ref scanner, result, out IntegerLiteral pi))
        {
            parsed = pi;
            return true;
        }
        parsed = null!;
        return false;
    }
}

public struct IntegerParser : IParser<IntegerLiteral>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out IntegerLiteral node)
       
    {
        var position = scanner.Position;
        DigitTerminalParser nonZero = new(DigitMode.ExceptZero);
        DigitTerminalParser digitParser = new();
        IntegerSuffixParser suffix = new();
        if (nonZero.Match(ref scanner))
        {
            scanner.Advance(1);
            while (digitParser.Match(ref scanner))
                scanner.Advance(1);
            var numPos = scanner.Position;
            if (suffix.Match(ref scanner, null!, out Suffix suf))
            {
                node = new(suf, long.Parse(scanner.Span[position..numPos]), scanner.GetLocation(position, scanner.Position));
                return true;
            }
            else
            {
                var memory = scanner.Memory[position..scanner.Position];
                node = new(new(32, false, true), long.Parse(memory.Span), new(scanner.Line, scanner.Column - memory.Length, memory));
                return true;
            }
        }
        node = new(new(), 0, new());
        scanner.Position = position;
        return false;
    }
}

public struct FloatParser : IParser<FloatLiteral>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out FloatLiteral node)
       
    {
        var position = scanner.Position;
        node = null!;
        FloatSuffixParser suffix = new();
        if (Terminals.Char('.', ref scanner))
        {
            scanner.Advance(1);
            while (Terminals.Digit(ref scanner))
                scanner.Advance(1);
            if (suffix.Match(ref scanner, result, out Suffix s))
            {
                scanner.Advance(1);
                node = new FloatLiteral(s, double.Parse(scanner.Span[position..scanner.Position]), new(scanner.Line, scanner.Column - (scanner.Position - position), scanner.Memory[position..scanner.Position]));
            }
            return true;
        }
        else if (Terminals.Digit(ref scanner, DigitMode.ExceptZero))
        {
            scanner.Advance(1);
            while (Terminals.Digit(ref scanner))
                scanner.Advance(1);
            Suffix s = new(32, true, true);
            if (Terminals.Char('.', ref scanner))
            {
                scanner.Advance(1);
                while (Terminals.Digit(ref scanner))
                    scanner.Advance(1);
            }
            else if (!suffix.Match(ref scanner, result, out s))
            {
                scanner.Position = position;
                return false;
            }
            var len = 0;
            foreach (var e in scanner.Span[position..scanner.Position])
                if (!char.IsDigit(e))
                    break;
                else
                    len += 1;
            node = new FloatLiteral(s, double.Parse(scanner.Span[position..len]), new(scanner.Line, scanner.Column - (scanner.Position - position), scanner.Memory[position..scanner.Position]));

            return true;
        }
        else if (Terminals.Digit(ref scanner, DigitMode.OnlyZero))
        {
            scanner.Advance(1);
            Suffix s = new(32, true, true);
            if (Terminals.Char('.', ref scanner))
            {
                scanner.Advance(1);
                while (Terminals.Digit(ref scanner))
                    scanner.Advance(1);
                if (!suffix.Match(ref scanner, result, out s))
                    s = new(32, true, true);
            }
            node = new FloatLiteral(s, double.Parse(scanner.Span[position..scanner.Position]), new(scanner.Line, scanner.Column - (scanner.Position - position), scanner.Memory[position..scanner.Position]));
            return true;
        }
        else 
        {
            scanner.Position = position;
            return false;
        }
    }
}
// public struct HexParser : IParser<HexLiteral>
// {
//     public bool Match(ref Scanner scanner, ParseResult result, out HexLiteral node)
//        
//     {
//         var position = scanner.Position;
//         DigitTerminalParser zero = new(DigitMode.OnlyZero);
//         DigitTerminalParser nonZero = new(DigitMode.ExceptZero);
//         DigitTerminalParser digitParser = new();
//         CharTerminalParser dot = new('.');
//         SetTerminalParser suffix = new("fdh");
//         if (dot.Match(ref scanner))
//         {
//             scanner.Advance(1);
//             while (digitParser.Match(ref scanner))
//                 scanner.Advance(1);
//         }
//         else if (nonZero.Match(ref scanner))
//         {
//             scanner.Advance(1);
//             bool hasDot = true;

//         }
//         throw new NotImplementedException();
//     }
// }