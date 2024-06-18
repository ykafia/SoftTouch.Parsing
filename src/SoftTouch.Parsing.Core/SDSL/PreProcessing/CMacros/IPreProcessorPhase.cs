namespace SoftTouch.Parsing.SDSL.PreProcessing.Macros;

public interface IPreProcessorPhase
{
    SDSLPreProcessor Apply(SDSLPreProcessor sdslpp);
}
