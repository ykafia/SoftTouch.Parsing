namespace SoftTouch.Parsing.SDSL;

public struct Scanner
{

    readonly int start = 0;
    // public string Code { get; } = code;
    public readonly ReadOnlySpan<char> Code => Memory.Span;
    public ReadOnlyMemory<char> Memory { get; internal set; }
    public int Position { get; set; } = 0;

    public readonly int Line => Code[..Position].Count('\n') + 1;
    public readonly int Column => Position - Code[..Position].LastIndexOf('\n') + 1;


    public readonly int End => Code.Length;
    public readonly bool IsEof => Position >= End;

    public Scanner(string code)
    {
        Memory = code.AsMemory();
    }

    public Scanner(ReadOnlyMemory<char> code)
    {
        Memory = code;
    }


    public int ReadChar()
    {
        var pos = Position;
        if (pos < End)
        {
            Position = pos + 1;
            return Code[pos];
        }
        return -1;
    }

    public readonly int Peek()
    {
        var pos = Position;
        return pos < End ? Code[pos] : -1;
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
                    if (Code[index++] != matchString[i])
                        return false;
                }
                return true;
            }
            else
            {
                for (int i = 0; i < matchString.Length; i++)
                {
                    if (char.ToLowerInvariant(Code[index++]) != char.ToLowerInvariant(matchString[i]))
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
            var slice = Code.Slice(index, length);
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
            if (Code[i] == '\n')
                lineCount++;
        }
        return lineCount + 1;
    }

    public readonly TextLocation GetLocation(int position, int length)
    {
        return new(Line, Column, Memory.Slice(position, length));
    }
}



