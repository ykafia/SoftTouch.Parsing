using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;


public struct NumberParser : IParser<NumberLiteral>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out NumberLiteral parsed, in ParseError? orError = null)
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
        if (orError is not null)
            result.Errors.Add(orError.Value);
        parsed = null!;
        return false;
    }
}

public struct IntegerParser : IParser<IntegerLiteral>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out IntegerLiteral node, in ParseError? orError = null)

    {
        var position = scanner.Position;
        IntegerSuffixParser suffix = new();
        if (Terminals.Digit(ref scanner, DigitMode.ExceptZero, advance: true))
        {
            while (Terminals.Digit(ref scanner, advance: true)) ;

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
        else if (Terminals.Char('0', ref scanner, advance: true) && !Terminals.Digit(ref scanner, DigitMode.All))
        {
            var memory = scanner.Memory[position..scanner.Position];
            node = new(new(32, false, true), 0, new(scanner.Line, scanner.Column - memory.Length, memory));
            return true;
        }
        else
        {
            node = null!;
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            return false;
        }
    }
}

public struct FloatParser : IParser<FloatLiteral>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out FloatLiteral node, in ParseError? orError = null)

    {
        var position = scanner.Position;
        node = null!;
        FloatSuffixParser suffix = new();
        if (Terminals.Char('.', ref scanner))
        {
            scanner.Advance(1);
            while (Terminals.Digit(ref scanner, advance: true)) ;

            if (suffix.Match(ref scanner, result, out Suffix s))
                node = new FloatLiteral(s, double.Parse(scanner.Span[position..scanner.Position]), new(scanner.Line, scanner.Column - (scanner.Position - position), scanner.Memory[position..scanner.Position]));
            return true;
        }
        else if (Terminals.Digit(ref scanner, DigitMode.ExceptZero, advance: true))
        {
            while (Terminals.Digit(ref scanner, advance: true)) ;
            Suffix s = new(32, true, true);
            if (Terminals.Char('.', ref scanner, advance: true))
            {
                while (Terminals.Digit(ref scanner, advance: true)) ;
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
            if (Terminals.Char('.', ref scanner, advance: true))
            {
                while (Terminals.Digit(ref scanner, advance: true))
                    if (!suffix.Match(ref scanner, result, out s))
                        s = new(32, true, true);
            }
            node = new FloatLiteral(s, double.Parse(scanner.Span[position..scanner.Position]), new(scanner.Line, scanner.Column - (scanner.Position - position), scanner.Memory[position..scanner.Position]));
            return true;
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            scanner.Position = position;
            return false;
        }
    }
}
public struct HexParser : IParser<HexLiteral>
{
    public readonly bool Match(ref Scanner scanner, ParseResult result, out HexLiteral node, in ParseError? orError = null)
    {
        node = null!;
        var position = scanner.Position;
        if (Terminals.Literal("0x", ref scanner, advance: true))
        {
            while (Terminals.Set("abcdefABCDEF", ref scanner, advance: true) || Terminals.Digit(ref scanner, advance: true)) ;

            ulong sum = 0;

            for (int i = 0; i < scanner.Position - position - 2; i += 1)
            {
                var v = Hex2int(scanner.Span[i]);
                var add = v * Math.Pow(16, i);
                if (ulong.MaxValue - sum < add)
                {
                    result.Errors.Add(new ParseError("Hex value bigger than ulong.", new(scanner, position)));
                    return false;
                }
            }
            node = new HexLiteral(sum, scanner.GetLocation(position, scanner.Position - position));
            return true;
        }
        else
        {
            if (orError is not null)
                result.Errors.Add(orError.Value);
            return false;
        }
    }

    static int Hex2int(char ch)
    {
        if (ch >= '0' && ch <= '9')
            return ch - '0';
        if (ch >= 'A' && ch <= 'F')
            return ch - 'A' + 10;
        if (ch >= 'a' && ch <= 'f')
            return ch - 'a' + 10;
        return -1;
    }
}