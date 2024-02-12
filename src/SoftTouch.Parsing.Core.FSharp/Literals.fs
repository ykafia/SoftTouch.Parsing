module Literals

open Scanning
open Terminals
open System
open AST

let integerSuffixes = [|
        "u8";  "i8";
        "u16"; "i16";
        "u32"; "i32";
        "u64"; "i64";
    |]
let floatSuffixes = 
    [|
        "f16";
        "f32";
        "f64";
    |]



let parseInt (span : inref<ReadOnlySpan<char>>) 
    = Int64.Parse(span)
let parseFloat (span : inref<ReadOnlySpan<char>>) 
    = Double.Parse(span)


let integerParser (scanner : byref<Scanner>) (number : NumberLiteral outref) = 
    let position = scanner.Position
    if nonZeroDigitTerminal &scanner true then
        while digitTerminal &scanner true do 
            () |> ignore
        let slice = scanner.BackSlice position
        number <- Integer (parseInt &slice, (scanner.GetBackLocation position))
    scanner.Position > position


let floatParser (scanner : byref<Scanner>) (number : NumberLiteral outref) = 
    let position = scanner.Position
    printfn "char is %c" scanner.Span[scanner.Position]
    
    if nonZeroDigitTerminal &scanner true then
        printfn "char is %c" scanner.Span[scanner.Position]
        while digitTerminal &scanner true do 
            printfn "char is %c" scanner.Span[scanner.Position]
            
        if charTerminal '.' &scanner true then
            while digitTerminal &scanner true do 
                printfn "char is %c" scanner.Span[scanner.Position]
            let slice = scanner.BackSlice position
            number <- Float (parseFloat &slice, (scanner.GetBackLocation position))
        else 
            scanner.Position <- position
    scanner.Position > position
        
    
let numberParse (scanner : byref<Scanner>) (number : NumberLiteral outref) =
    let mutable result = false
    if floatParser &scanner &number then
        result <- true
    else if integerParser &scanner &number then
        result <- true
    result