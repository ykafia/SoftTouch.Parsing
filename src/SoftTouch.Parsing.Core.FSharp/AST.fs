module AST
open System



type TextLocation =
    struct 
        val Position : int32
        val Length : int32
        val Text : ReadOnlyMemory<char>
        new(position, length, text) = {Text = text; Position = position; Length = length}
        override this.ToString() = 
            $"\"{this.Text.ToString()}\""
    end

let emptyInfo = new TextLocation( 0,0, ReadOnlyMemory<char>.Empty)

type Expression =
    | Number of NumberLiteral
    | Variable of string * TextLocation
    | Ternary of Operator * Expression * Expression * Expression * TextLocation
    | Binary of Operator * Expression * Expression * TextLocation
    | Prefix of Prefix
    | Postfix of Postfix
and Prefix =
    | Cast of Expression * string * TextLocation
    | Increment of Expression * TextLocation
    | Decrement of Expression * TextLocation
    | Not of Expression * TextLocation
    | BNot of Expression * TextLocation
and Postfix =
    | Accessor of Expression * Expression * TextLocation
    | Increment of Expression * TextLocation
    | Decrement of Expression * TextLocation
    | Indexer of Expression * Expression * TextLocation
and NumberLiteral =
    | Integer of int64 * TextLocation
    | Float of float * TextLocation
and Operator =
    | Add
    | Subtract
    | Multiply
    | Divide
    | Mod
    | LeftShift
    | RightShift
    | Greater
    | Less
    | GreaterEqual
    | LessEqual
    | BOr
    | BAnd
    | BXor
    | Or
    | And