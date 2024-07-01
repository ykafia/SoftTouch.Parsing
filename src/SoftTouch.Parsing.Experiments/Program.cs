// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Runtime.InteropServices;
using SoftTouch.Parsing.SDSL;
using SoftTouch.Parsing.SDSL.AST;
using SoftTouch.Parsing.SDSL.PreProcessing;


// Creating a scanner
// for parsing code
var scanner = new Scanner("");

// var result = Grammar.Match<PostfixParser, Expression>("machin.chose().hello.world[3]-2*(3+ hello.world())");
// var result = Grammar.Match<ExpressionParser, Expression>("5 ?2: machin.  chose()  .   hello.world [  3 ]  -   2 *(3+ hello.world())");
// var path = @"C:\Users\youness_kafia\Documents\dotnetProjs\SoftTouch.Parsing\assets";
var path = @"C:\Users\kafia\source\repos\ykafia\SoftTouch.Parsing\assets";
Directory.SetCurrentDirectory(path);
var file = File.ReadAllText("./SDSL/Commented.sdsl");
var result = SDSLParser.Parse(file);
if(result.AST is not null)
    Console.WriteLine(result.AST);
if(result.Errors.Any())
    Console.WriteLine(result.Errors[0]);



// file = Grammar.MatchTyped<CodeNodeParsers, CodeSnippets>(file).AST?.ToCode() ?? "";
// Parse(file);

// static int Parse(string file)
// {
//     var x = 0;
//     // var uncommented = Grammar.MatchTyped<CodeNodeParsers, CodeSnippets>(file).AST?.ToCode();
//     // if (uncommented != null)
//     // {
//         var result = Grammar.Match<ShaderFileParser, ShaderFile>(file);
//         if (result.AST is not null)
//             x++;
//         // foreach (var e in result.Errors)
//         //     Console.WriteLine(e);
//     // }
//     return x;
// }
// var sw = new Stopwatch();
// foreach(var e in Enumerable.Range(0, 100))
// {
//     Parse(file);
// }

// sw.Start();
// Parse(file);
// sw.Stop();
// Console.WriteLine(sw.Elapsed.TotalMicroseconds + "µs");
