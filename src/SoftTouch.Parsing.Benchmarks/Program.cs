using SDSL.Parsing.Grammars.SDSL;


var grammar = new SDSLGrammar();


grammar.Inner = grammar.OrExpression;

grammar.Match("machin.chose[3].something.else[3]");

