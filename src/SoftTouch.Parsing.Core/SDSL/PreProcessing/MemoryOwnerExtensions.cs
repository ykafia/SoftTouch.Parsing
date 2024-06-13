using CommunityToolkit.HighPerformance.Buffers;

namespace SoftTouch.Parsing.SDSL.PreProcessing;


public static class MemoryOwnerExtensions
{
    public static MemoryOwner<T> Resize<T>(this MemoryOwner<T> owner, int size)
    {
        var result = MemoryOwner<T>.Allocate(owner.Length + size, AllocationMode.Clear);
        owner.Span[..Math.Min(result.Length, owner.Length)].CopyTo(result.Span);
        owner.Dispose();
        return result;
    }
}