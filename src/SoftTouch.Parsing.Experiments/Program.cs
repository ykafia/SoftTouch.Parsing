// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using SoftTouch.Parsing.SDSL;
using SoftTouch.Parsing.SDSL.AST;

// var result = Grammar.Match<PostfixParser, Expression>("machin.chose().hello.world[3]-2*(3+ hello.world())");
// var result = Grammar.Match<ExpressionParser, Expression>("5 ?2: machin.  chose()  .   hello.world [  3 ]  -   2 *(3+ hello.world())");
var path = @"C:\Users\youness_kafia\Documents\dotnetProjs\SoftTouch.Parsing\assets";
Directory.SetCurrentDirectory(path);
// var toparse = "var a = 5 ?2: (machin.  chose()  .   hello.world)[  3 ]++  -   2 *(3+ hello.world());";
var uncommented = Grammar.MatchTyped<CodeNodeParsers, CodeSnippets>(File.ReadAllText("./SDSL/MyShader.sdsl")).AST?.ToCode();
// Console.WriteLine(uncommented);
if (uncommented != null)
{
    var result = Grammar.Match<ShaderClassParsers, ShaderClass>(uncommented);
    if (result.AST is not null)
        Console.WriteLine(result.AST);
    foreach (var e in result.Errors)
        Console.WriteLine(e);
}

TryOldSDSL();
static void TryOldSDSL()
{
    var parser = new SDSL.Parsing.Grammars.SDSL.SDSLGrammar();
    var match = parser.Match(
"""
shader MyShader
{
    void MyMethod()
    {
        int a = 0;
        int b = (a - 10 / 3 ) * 32 +( 8 % streams.color.Normalize() + 2 << 5);
    }
}
"""
    );
    var program = SDSL.Parsing.AST.Shader.ShaderToken.Tokenize(match);
    var x = 0;
}