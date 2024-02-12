module Scanning
open System
open AST

let rec matchSensitiveChar (i : int32) (index : int32) (span : inref<ReadOnlySpan<char>>) (matchString : string): int32 =
    if span[index + i] = matchString.[i] then
        matchSensitiveChar (i + 1) index &span matchString
    else
        i

let rec matchChar (i : int32) (index : int32) (span : inref<ReadOnlySpan<char>>) (matchString : string): int32 =
    if Char.ToLowerInvariant(span[index + i]) <> Char.ToLowerInvariant(matchString.[i]) then
        matchChar (i + 1) index &span matchString
    else
        i

type Scanner =
    struct
        val Code : string
        val mutable Position : int32

        new(code : string) = { Code = code; Position = 0 }
        member this.Span : ReadOnlySpan<char> =
            this.Code.AsSpan()
        member this.Memory : ReadOnlyMemory<char> =
            this.Code.AsMemory()
        member this.IsEOL : bool =
            this.Position > this.Code.Length
        member this.End : int32 =
            this.Code.Length
        

        member this.Peek() : int32= 
            if this.Position + 1 < this.End && int this.Span[this.Position + 1] > 0 then  
                int this.Span[this.Position + 1]
            else 
                -1

        member this.Advance (length : int32) =
            let pos = this.Position;
            let newPos = pos + length;
            let mutable result = false
            if newPos <= this.End then
                this.Position <- newPos
                result <- true
            result

        member this.ReadString (matchString : string) (caseSensitive : bool) : bool =
            let endstring = this.Position + matchString.Length
            let mutable result = true

            if endstring <= this.End then
                let span = this.Span
                if caseSensitive then
                    if (matchSensitiveChar 0 this.Position &span matchString) < matchString.Length then
                        result <- false
                else
                    if (matchChar 0 this.Position &span matchString) < matchString.Length then
                        result <- false

            result
        
        member this.Slice position length = 
            this.Span.Slice(position, length)
        member this.BackSlice position = 
            this.Span.Slice(position, this.Position - position + 1)
        
        member this.GetLocation position length = 
            new TextLocation(position, length, this.Memory.Slice(position, length))
        member this.GetBackLocation position = 
            new TextLocation(position, this.Position - position, this.Memory.Slice(position, this.Position - position + 1))
    end