namespace SoftTouch.Parsing.SDSL;


public struct Scanner(string code)
{
    public string Code { get; } = code;
    public readonly ReadOnlyMemory<char> Memory => Code.AsMemory();
    public readonly ReadOnlySpan<char> Span => Code.AsSpan();
    public int Position { get; set; } = 0;
    readonly int end = code.Length;
    readonly int start = 0;


    public readonly bool IsEof => Position >= end;


    public int ReadChar()
    {
        var pos = Position;
        if (pos < end)
        {
            Position = pos + 1;
            return Span[pos];
        }
        return -1;
    }

    public readonly int Peek()
    {
        var pos = Position;
        return pos < end ? Code[pos] : -1;
    }
    public readonly ReadOnlySpan<char> Peek(int size)
        => Position < end ? Slice(Position,size) : [];

    public int Advance(int length)
    {
        var pos = Position;
        var newPos = pos + length;
        if (newPos <= end)
        {
            Position = newPos;
            return pos;
        }
        return -1;
    }

    public bool ReadString(string matchString, bool caseSensitive)
    {
        var index = Position;
        var endstring = index + matchString.Length;
        if (endstring <= end)
        {
            if (caseSensitive)
            {
                for (int i = 0; i < matchString.Length; i++)
                {
                    if (Span[index++] != matchString[i])
                        return false;
                }
                Position = endstring;
                return true;
            }
            else
            {
                for (int i = 0; i < matchString.Length; i++)
                {
                    if (char.ToLowerInvariant(Span[index++]) != char.ToLowerInvariant(matchString[i]))
                        return false;
                }
                Position = endstring;
                return true;
            }
        }
        return false;
    }

    public readonly ReadOnlySpan<char> Slice(int index, int length)
    {
        if (index < end)
        {
            length = Math.Min(index + length, end) - index;
            var slice = Span.Slice(index, length);
            return slice;
        }
        return [];
    }

    public readonly int LineAtIndex(int index)
    {
        int lineCount = 0;
        var max = Math.Min(end, index);
        for (int i = start; i < max; i++)
        {
            if (Span[i] == '\n')
                lineCount++;
        }
        return lineCount + 1;
    }
}