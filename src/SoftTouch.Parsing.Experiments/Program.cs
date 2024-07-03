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
// var m = Grammar.Match<ExpressionParser, Expression>("machin.chose().hello.world[3]-2*(3+ hello.world())");
var path = @"C:\Users\youness_kafia\Documents\dotnetProjs\SoftTouch.Parsing\assets";



// var path = @"C:\Users\kafia\source\repos\ykafia\SoftTouch.Parsing\assets";
Directory.SetCurrentDirectory(path);
var file = File.ReadAllText("./SDSL/Commented.sdsl");
var parent = File.ReadAllText("./SDSL/Parent.sdsl");
var x = 0;
var result = SDSLParser.Parse(parent);
if(result.AST is not null)
    Console.WriteLine(result.AST);
if(result.Errors.Count != 0)
    Console.WriteLine(result.Errors[0]);
