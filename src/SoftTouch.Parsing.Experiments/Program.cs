// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using SoftTouch.Parsing.SDSL;
using SoftTouch.Parsing.SDSL.AST;

var result = Grammar.Match<PostfixParser, Expression>("machin.chose().hello.world[3]++ - 2 * (3+ hello.world())");
if(result.AST is not null)
    Console.WriteLine(result.AST);
foreach(var e in result.Errors)
    Console.WriteLine(e);
