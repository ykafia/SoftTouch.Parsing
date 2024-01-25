using SoftTouch.Parsing.SDSL.AST;

namespace SoftTouch.Parsing.SDSL;

public record struct Result<TResult, TError>(TResult? Ok = null, TError? Err = null)
    where TResult : Node
    where TError : struct
{
    public static implicit operator Result<TResult,TError>(TResult res) => new(res, null);
    public static implicit operator Result<TResult,TError>(TError err) => new(null, err);
}