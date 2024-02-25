using System.Text;
using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public class PreProcessor(Dictionary<string,string>? parameters = null)
{
    public List<TextLocation> CodeFragments { get; } = [];
    public Dictionary<string, string> Objects { get; } = parameters ?? [];

    public void PreProcess(string code)
    {
        var scanner = new Scanner(code);
        while(!scanner.IsEof)
        {
            while(Terminals.Char(' ', ref scanner, advance: true));
            if(Terminals.Literal("#if ", ref scanner, advance: true))
            {
                while (Terminals.Char(' ', ref scanner, advance: true));
            }
        }
    }

}