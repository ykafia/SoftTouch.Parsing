// using System.Text;
// using BenchmarkDotNet.Attributes;
// using SoftTouch.Parsing.SDSL;
// using SoftTouch.Parsing.SDSL.AST;

// namespace SoftTouch.Parsing.Benchmarks;


// [MemoryDiagnoser]
// public class ExpressionBench
// {
//     string expression = "";

//     public ExpressionBench()
//     {
//         var rand = new Random();
//         var b = new StringBuilder();
//         for(int i = 0; i < 1000; i++)
//         {
//             b.Append(rand.Next(1000, 9999)).Append(' ').Append(
//                 (i % 3) switch {
//                     0 => '+',
//                     1 => '-',
//                     _ => '*',
//                 }
//             ).Append(' ');
//         }
//         expression = b.ToString();
//     }

//     [Benchmark]
//     public void RecursiveParse()
//     {
//         Grammar.Match<AdditionParser, Expression>(expression);
//     }

//     [Benchmark]
//     public void PrattParse()
//     {
//         Grammar.Match<OtherAdditionParser, Expression>(expression);
//     }
// }