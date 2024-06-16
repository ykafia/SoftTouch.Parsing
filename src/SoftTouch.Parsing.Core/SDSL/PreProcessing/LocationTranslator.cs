using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;

namespace SoftTouch.Parsing.SDSL.PreProcessing;



public record struct TextLink(Range Origin, Range Processed);


public class LocationTranslator(Memory<char> origin, Memory<char> processed)
{
    public List<TextLink> Links { get; set; } = [];
    public Memory<char> Origin { get; set; } = origin;
    public Memory<char> Processed { get; set; } = processed;


    /// <summary>
    /// Gets the list of text locations that translate the range chosen to the original file.
    /// </summary>
    /// <value></value>
    public MemoryOwner<TextLocation> this[Range range]
    {
        get 
        {
            (var start, var length) = range.GetOffsetAndLength(Processed.Length);
            var end = start + length;
            using var result = MemoryOwner<TextLocation>.Empty;
            foreach(var l in Links)
            {
                (var po, var pl) = l.Processed.GetOffsetAndLength(Processed.Length);
                if(start > po && start < po + pl)
                {
                    
                }
            }
            return Origin.Span[..];
        }
    }
}