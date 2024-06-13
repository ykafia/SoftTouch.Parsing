using CommunityToolkit.HighPerformance.Buffers;

namespace SoftTouch.Parsing.SDSL.PreProcessing;

public struct CommentPhase() : IPreProcessorPhase
{
    public SDSLPreProcessor Apply(SDSLPreProcessor sdslpp)
    {
        var frame = new CodeFrame();
        
        return sdslpp;
    }
}
