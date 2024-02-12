// For more information see https://aka.ms/fsharp-console-apps
open Literals
open Scanning
open AST

let mutable scanner = new Scanner("21345")
let mutable number = Integer(0, emptyInfo)
match numberParse &scanner &number with
| true -> printfn "%A" number
| false -> printfn "Error"

