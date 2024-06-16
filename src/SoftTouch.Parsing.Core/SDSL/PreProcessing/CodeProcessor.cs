using CommunityToolkit.HighPerformance.Buffers;

namespace SoftTouch.Parsing.SDSL.PreProcessing;

public class SDSLPreProcessor() : IDisposable
{
    public List<CodeFrame> CodeFrames { get; set; } = [];

    public void Run() =>
        Apply<CommentPhase>();

    public SDSLPreProcessor Apply<TPhase>() 
        where TPhase : struct, IPreProcessorPhase
        => new TPhase().Apply(this);

    public void Dispose()
    {
        CodeFrames.ForEach(static x => x.Dispose());
        CodeFrames.Clear();
    }
}