using System.Numerics;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;



public class PreProcessor(Dictionary<string, Literal>? parameters = null) : IDisposable
{
    public List<TextLocation> CodeFragments { get; } = [];
    public Dictionary<string, Literal> Variables { get; } = parameters ?? [];
    public CodeBuffer original = new();
    public PreProcessedCodeBuffer processed = new(new());

    public PreProcessor With(string file)
    {
        original.Add(file);
        processed.Add(file);
        return this;
    }
    public PreProcessor With(ReadOnlyMemory<char> file)
    {
        original.Add(file);
        processed.Add(file);
        return this;
    }
    public PreProcessor With(ReadOnlySpan<char> file)
    {
        original.Add(file);
        processed.Add(file);
        return this;
    }

    public string PreProcess()
    {
        // Comment preprocess
        var commentLine = new LiteralTerminalParser("//");
        var commentBlockStart = new LiteralTerminalParser("/*");
        var commentBlockEnd = new LiteralTerminalParser("*/");

        var tokenized = new TokenizedCode()
        {
            Processed = processed
        };
    
        var scanner = new Scanner(tokenized);

        while (!scanner.IsEof)
        {
            var curPos = scanner.Position;
            if (
                CommonParsers.Until<LiteralTerminalParser, LiteralTerminalParser>(
                    ref scanner,
                    commentLine,
                    commentBlockStart
                )
            )
            {
                if (Terminals.Literal("//", ref scanner))
                {
                    var pos = scanner.Position;
                    CommonParsers.Until(ref scanner, '\n', advance: true);
                    processed.Trim(pos, scanner.Position - pos);
                }
                else if (Terminals.Literal("/*", ref scanner))
                {
                    var pos = scanner.Position;
                    CommonParsers.Until(ref scanner, "*/", advance: true);
                    processed.Trim(pos, scanner.Position - pos);
                }
                else throw new NotImplementedException();
            }
        }
        // How to parse 
        scanner.Position = 0;
        var result = new ParseResult();

        // while(!scanner.IsEof)
        // {
        //     // Try parse directive
        //     // If match, evaluate directive
        //     // If evaluate to true continue
        //     // If not, skip code block until next directive
        //     var pos = scanner.Position;
        //     CommonParsers.Spaces0(ref scanner, result, out _, onlyWhiteSpace: true);
        //     if(Terminals.Literal("#define", ref scanner, advance: true))
        //     {
        //         // Replace all occurences of the define value with the new value
        //     }
        //     else if (Terminals.Literal("#ifdef", ref scanner, advance: true))
        //     {
        //         if(
        //             CommonParsers.Spaces1(ref scanner, result, out _, onlyWhiteSpace: true)
        //             && LiteralsParser.Identifier(ref scanner, result, out var identifier)
        //             && Terminals.EOL(ref scanner, advance: true)
        //         )
        //         {
        //             if(Variables.ContainsKey(identifier.Name))
        //             {

        //             }
        //         }
        //         else throw new NotImplementedException();
        //         // If the value is defined then skip to the next directive
        //     }
        //     else if (Terminals.Literal("#ifndef", ref scanner, advance: true))
        //     {

        //     }
        //     else if(Terminals.Literal("#if", ref scanner, advance: true))
        //     {

        //     }


        // }
        // var sansComment = sansCommentBuilder.ToString();
        // var macroScanner = new Scanner(sansComment);
        // var result = new ParseResult();
        // new ShaderFileParser().Match(ref shaderScanner, result, out var ast);
        // {
            // while (!scanner.IsEof)
            // {
            //     CommonParsers.Spaces0(ref scanner, result, out _, onlyWhiteSpace: true);
            //     if (Terminals.Literal("#ifdef", ref scanner, advance: true))
            //     {
            //         CommonParsers.Spaces0(ref scanner, result, out _, onlyWhiteSpace: true);
            //         DirectiveExpressionParser.Expression(ref scanner, result, out var expression);
            //         if(Evaluate(expression))
            //         {
            //             // Add code;
            //         }
            //     }
            // }
        // }
        return processed.Span.ToString();
    }

    public bool Evaluate(Expression expression)
    {
        return expression switch
        {
            BinaryExpression bin => Evaluate(bin) switch
            {
                BoolLiteral b => b.Value,
                NumberLiteral n => n.IntValue > 0,
                _ => throw new NotImplementedException($"Cannot evaluate expression {bin}")
            },
            BoolLiteral b => b.Value,
            NumberLiteral n => n.IntValue > 0,
            _ => throw new NotImplementedException($"Cannot evaluate expression {expression}")
        };
    }

    public Literal Evaluate(BinaryExpression expression)
    {
        Literal left = null!;
        Literal right = null!;
        if (expression.Left is BinaryExpression bil)
            left = Evaluate(bil);
        else if (expression.Left is Identifier lid)
            left = Variables[lid.Name];
        else if (expression.Left is Literal llit)
            left = llit;
        else throw new NotImplementedException($"Expression {left} cannot be evaluated");
        if (expression.Right is BinaryExpression bir)
            right = Evaluate(bir);
        else if (expression.Right is Identifier rid)
            right = Variables[rid.Name];
        else if (expression.Right is Literal rlit)
            right = rlit;
        else throw new NotImplementedException($"Expression {right} cannot be evaluated");



        return (left, right, expression.Op) switch
        {
            (NumberLiteral l, NumberLiteral r, Operator.Plus) => (FloatLiteral)(l.DoubleValue + r.DoubleValue),
            (NumberLiteral l, NumberLiteral r, Operator.Minus) => (FloatLiteral)(l.DoubleValue - r.DoubleValue),
            (NumberLiteral l, NumberLiteral r, Operator.Mul) => (FloatLiteral)(l.DoubleValue * r.DoubleValue),
            (NumberLiteral l, NumberLiteral r, Operator.Div) => (FloatLiteral)(l.DoubleValue / r.DoubleValue),
            (NumberLiteral l, NumberLiteral r, Operator.Mod) => (FloatLiteral)(l.DoubleValue % r.DoubleValue),
            (NumberLiteral l, NumberLiteral r, Operator.LeftShift) => (FloatLiteral)(l.IntValue << r.IntValue),
            (NumberLiteral l, NumberLiteral r, Operator.RightShift) => (FloatLiteral)(l.IntValue >> r.IntValue),
            (NumberLiteral l, NumberLiteral r, Operator.AND) => (FloatLiteral)(l.IntValue & r.IntValue),
            (NumberLiteral l, NumberLiteral r, Operator.OR) => (FloatLiteral)(l.IntValue | r.IntValue),
            (NumberLiteral l, NumberLiteral r, Operator.XOR) => (FloatLiteral)(l.IntValue | r.IntValue),
            (NumberLiteral l, NumberLiteral r, Operator.Equals) => new BoolLiteral(l.IntValue == r.IntValue, new()),
            (NumberLiteral l, NumberLiteral r, Operator.Greater) => new BoolLiteral(l.IntValue > r.IntValue, new()),
            (NumberLiteral l, NumberLiteral r, Operator.GreaterOrEqual) => new BoolLiteral(l.IntValue >= r.IntValue, new()),
            (NumberLiteral l, NumberLiteral r, Operator.Lower) => new BoolLiteral(l.IntValue < r.IntValue, new()),
            (NumberLiteral l, NumberLiteral r, Operator.LowerOrEqual) => new BoolLiteral(l.IntValue <= r.IntValue, new()),
            (BoolLiteral l, BoolLiteral r, Operator.Equals or Operator.LogicalAND) => new BoolLiteral(l.Value == r.Value, new()),
            (BoolLiteral l, BoolLiteral r, Operator.NotEquals) => new BoolLiteral(l.Value != r.Value, new()),
            (BoolLiteral l, BoolLiteral r, Operator.LogicalOR) => new BoolLiteral(l.Value || r.Value, new()),
            _ => throw new NotImplementedException($"Cannot evaluate expression {left}{expression.Op}{right}")
        };
    }

    public void Dispose() => processed.Dispose();
}