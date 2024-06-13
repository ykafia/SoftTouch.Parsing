namespace SoftTouch.Parsing.SDSL.PreProcessing;

public interface IPreProcessorPhase
{
    SDSLPreProcessor Apply(SDSLPreProcessor sdslpp);
}
