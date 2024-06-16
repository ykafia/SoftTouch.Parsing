using CommunityToolkit.HighPerformance.Buffers;

namespace SoftTouch.Parsing.SDSL.PreProcessing;


public struct CommentPreProcessor
{
    public ReadOnlyMemory<char> Original { get; set; }
    public MemoryOwner<char> Processed { get; set; } = MemoryOwner<char>.Empty;
    public List<TextLink> Links { get; } = [];

    public CommentPreProcessor(ReadOnlyMemory<char> originalFile)
    {
        Original = originalFile;
        Process();
    }

    internal void Process()
    {
        var scanner = new Scanner<ScannableReadOnlyMemory>(new(Original));
        var started = false;
        var lastPos = 0;
        while (!scanner.IsEof)
        {
            CommonParsers.Until(ref scanner, ["//", "/*", "\""]);
            if (!started)
                started = true;
            Add(lastPos..scanner.Position);
            lastPos = scanner.Position;
            if (Terminals.Literal("//", ref scanner))
            {
                CommonParsers.Until(ref scanner, '\n', advance: true);
                lastPos = scanner.Position;
                Add([' ']);
            }
            else if (Terminals.Literal("/*", ref scanner))
            {
                CommonParsers.Until(ref scanner, "*/", advance: true);
                lastPos = scanner.Position;
                Add([' ']);
            }
            else if (Terminals.Literal("\"", ref scanner))
            {
                CommonParsers.Until(ref scanner, "\"", advance: true);
                Add(lastPos..scanner.Position);
                lastPos = scanner.Position;
            }
        }
    }

    internal void Add(Range range)
    {
        (_, var length) = range.GetOffsetAndLength(Original.Length);
        Processed = Processed.Add(Original.Span[range]);
        Links.Add(new(range, (Processed.Length - length)..Processed.Length));

    }
    internal void Add(Span<char> span)
    {
        Processed = Processed.Add(span);
    }

    /// <summary>
    /// Gets the list of text locations that translate the range chosen to the original file.
    /// </summary>
    /// <value></value>
    public readonly TextLocation GetOriginalRanges(Range range)
    {
        var (start, length) = range.GetOffsetAndLength(Processed.Length);
        var end = start + length;
        var outputStart = -1;
        foreach (var link in Links)
        {
            var (linkStart, linkLength) = link.Processed.GetOffsetAndLength(Processed.Length);
            var linkEnd = linkStart + linkLength;
            (var linkOriginalStart, var linkOriginalLength) = link.Origin.GetOffsetAndLength(Original.Length);
            var linkOriginalEnd = linkOriginalStart + linkOriginalLength;
            var realStart = linkOriginalStart + (start - linkStart);
            if (outputStart == -1 && start >= linkStart && start < linkEnd)
            {
                outputStart = realStart;
            }
            if (end <= linkEnd)
            {
                var outputEnd = linkOriginalEnd - (linkEnd - end);
                return new(Original, outputStart..outputEnd);
            }
        }
        return new(Original, 0..0);
    }
}