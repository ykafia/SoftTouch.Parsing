module Terminals 
open Scanning
open System

let charTerminal (c : char) (scanner : byref<Scanner>) (advance : bool): bool =
    let mutable result = false
    if scanner.Peek() = int32 c then
        result <- true
        if advance then 
            scanner.Position <- scanner.Position + 1
    result

let digitTerminal (scanner : byref<Scanner>) (advance : bool): bool =
    let mutable result = false
    if scanner.Peek() > 0 && scanner.Peek() |> char |> Char.IsDigit then
        result <- true
        if advance then 
            scanner.Position <- scanner.Position + 1
    result

let nonZeroDigitTerminal (scanner : byref<Scanner>) (advance : bool): bool =
    let mutable result = false
    if scanner.Peek() > 0 && scanner.Peek() |> char |> Char.IsDigit && (scanner.Peek() |> char <> '0') then
        result <- true
        if advance then 
            scanner.Position <- scanner.Position + 1
    result

let letterTerminal (scanner : byref<Scanner>) (advance : bool): bool =
    let mutable result = false
    if scanner.Peek() > 0 && scanner.Peek() |> char |> Char.IsLetter then
        result <- true
        if advance then 
            scanner.Position <- scanner.Position + 1
    result

let letterOrDigitTerminal (scanner : byref<Scanner>) (advance : bool): bool =
    let mutable result = false
    if scanner.Peek() > 0 && scanner.Peek() |> char |> Char.IsLetterOrDigit then
        result <- true
        if advance then 
            scanner.Position <- scanner.Position + 1
    result

let literalTerminal (terminal : string) (scanner : byref<Scanner>) (advance : bool): bool =
    let mutable result = false
    if scanner.ReadString terminal true 
        then
        result <- true
        if advance then 
            scanner.Position <- scanner.Position + 1
    result

let literalUnsensitiveTerminal (terminal : string) (scanner : byref<Scanner>) (advance : bool): bool =
    let mutable result = false
    if scanner.ReadString terminal false 
        then
        result <- true
        if advance then 
            scanner.Position <- scanner.Position + 1
    result