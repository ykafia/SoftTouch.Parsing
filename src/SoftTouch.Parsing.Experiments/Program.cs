// See https://aka.ms/new-console-template for more information
using SoftTouch.Parsing.Core;
using SoftTouch.Parsing.Core.Parsers;

Console.WriteLine("Hello, World!");


var letter = Terminals.Letter();

var text = "h";

var a = new ParserArgs(text);

letter.InnerParse(ref a);