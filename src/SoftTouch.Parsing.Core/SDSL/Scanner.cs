namespace SoftTouch.Parsing.SDSL;

public struct Scanner
{

    readonly int start = 0;
    // public string Code { get; } = code;
    public readonly ReadOnlySpan<char> Span => StringCode != null ? StringCode.Value.Span : ProcessedCode != null ? ProcessedCode.Value.Span : throw new NotImplementedException();
    public readonly ReadOnlyMemory<char> Memory => StringCode != null ? StringCode.Value.Memory : ProcessedCode != null ? ProcessedCode.Value.Memory : throw new NotImplementedException();
    StringCode? StringCode { get; }
    TokenizedCode? ProcessedCode { get; }
    public int Position { get; set; } = 0;

    public readonly int Line => Span[..Position].Count('\n') + 1;
    public readonly int Column => Position - Span[..Position].LastIndexOf('\n') + 1;



    public readonly int End => Span.Length;
    public readonly bool IsEof => Position >= End;


    public Scanner(string code)
    {
        StringCode = code;
        ProcessedCode = null;
    }
    public Scanner(ReadOnlyMemory<char> code)
    {
        StringCode = code;
        ProcessedCode = null;
    }
    public Scanner(StringCode code)
    {
        StringCode = code;
        ProcessedCode = null;
    }
    public Scanner(TokenizedCode code)
    {
        ProcessedCode = code;
        StringCode = null;
    }

    public int ReadChar()
    {
        var pos = Position;
        if (pos < End)
        {
            Position = pos + 1;
            return Span[pos];
        }
        return -1;
    }

    public readonly int Peek()
    {
        var pos = Position;
        return pos < End ? Span[pos] : -1;
    }
    public readonly ReadOnlySpan<char> Peek(int size)
        => Position < End ? Slice(Position, size) : [];

    public int Advance(int length)
    {
        var pos = Position;
        var newPos = pos + length;
        if (newPos <= End)
        {
            Position = newPos;
            return pos;
        }
        return -1;
    }

    public readonly bool ReadString(string matchString, bool caseSensitive)
    {
        var index = Position;
        var Endstring = index + matchString.Length;
        if (Endstring <= End)
        {
            if (caseSensitive)
            {
                for (int i = 0; i < matchString.Length; i++)
                {
                    if (Span[index++] != matchString[i])
                        return false;
                }
                return true;
            }
            else
            {
                for (int i = 0; i < matchString.Length; i++)
                {
                    if (char.ToLowerInvariant(Span[index++]) != char.ToLowerInvariant(matchString[i]))
                        return false;
                }
                return true;
            }
        }
        return false;
    }

    public readonly ReadOnlySpan<char> Slice(int index, int length)
    {
        if (index < End)
        {
            length = Math.Min(index + length, End) - index;
            var slice = Span.Slice(index, length);
            return slice;
        }
        return [];
    }

    public readonly int LineAtIndex(int index)
    {
        int lineCount = 0;
        var max = Math.Min(End, index);
        for (int i = start; i < max; i++)
        {
            if (Span[i] == '\n')
                lineCount++;
        }
        return lineCount + 1;
    }

    public readonly TextLocation GetLocation(int position, int length)
    {
        return new(Line, Column, Memory.Slice(position, length));
    }
}



