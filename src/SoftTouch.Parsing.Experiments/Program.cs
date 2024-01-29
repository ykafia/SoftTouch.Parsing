// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using SoftTouch.Parsing.SDSL;
using SoftTouch.Parsing.SDSL.AST;


Console.WriteLine(char.IsWhiteSpace('\n'));
var result = Grammar.Match<NumberLiteral>("1h");
if(result.AST is not null)
    Console.WriteLine(result.AST);
else
    foreach(var e in result.Errors)
        Console.WriteLine(e);
