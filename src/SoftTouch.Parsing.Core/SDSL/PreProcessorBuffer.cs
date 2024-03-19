using System.Dynamic;
using System.Numerics;
using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using SoftTouch.Parsing.SDSL;

namespace SoftTouch.Parsing;


public class CodeBuffer : IDisposable
{
    MemoryOwner<char> _owner;
    public int Length { get; protected set; } = 0;
    public Span<char> Span => _owner.Span[..Length];
    public Memory<char> Memory => _owner.Memory[..Length];

    public ref char this[Index idx] => ref Span[idx];
    public Span<char> this[Range range] => Span[range];

    public CodeBuffer()
    {
        _owner = MemoryOwner<char>.Empty;
        Length = 0;
    }
    public CodeBuffer(string firstFile)
    {
        var len = (int)BitOperations.RoundUpToPowerOf2((uint)firstFile.Length);
        _owner = MemoryOwner<char>.Allocate(len, AllocationMode.Clear);
        Length = len;
        firstFile.AsSpan().CopyTo(Span);
    }

    public void Add(string file)
    {
        Expand(file.Length);
        Length += file.Length;
        file.AsSpan().CopyTo(Span[(Length - file.Length)..]);
    }
    public void Add(ReadOnlySpan<char> file)
    {
        Expand(file.Length);
        Length += file.Length;
        file.CopyTo(Span[(Length - file.Length)..]);
    }
    public void Add(ReadOnlyMemory<char> file)
    {
        Expand(file.Length);
        Length += file.Length;
        file.Span.CopyTo(Span[(Length - file.Length)..]);
    }

    protected void Expand(int size)
    {
        var newSize = Length + size;
        if (newSize > _owner.Length)
        {
            var n = MemoryOwner<char>.Allocate((int)BitOperations.RoundUpToPowerOf2((uint)newSize), AllocationMode.Clear);
            Span.CopyTo(n.Span);
            _owner.Dispose();
            _owner = n;
        }
    }

    public void Dispose()
        => _owner.Dispose();

}


public sealed class PreProcessedCodeBuffer() : CodeBuffer, IDisposable
{

    public void Remove(int start, int length)
    {
        Span[(start + length)..].CopyTo(Span[start..]);
        Length -= length;
    }

    public void Replace(int position, int length, string value)
        => Replace(position, length, value.AsSpan());
    public void Replace(int position, int length, Memory<char> value)
        => Replace(position, length, value.Span);
    public void Replace(int position, int length, ReadOnlyMemory<char> value)
        => Replace(position, length, value.Span);
    public void Replace(int position, int length, Span<char> value)
        => Replace(position, length, (ReadOnlySpan<char>)value);
    public void Replace(int position, int length, ReadOnlySpan<char> value)
    {
        if (value.Length > length)
        {
            Expand(value.Length - length);
            Length += value.Length - length;
        }

        Span[(position + length)..].CopyTo(Span[(position + value.Length)..]);
        value.CopyTo(Span[position..(position + value.Length)]);

        if (value.Length < length)
            Length -= length - value.Length;
    }
    public void Trim(int start, int length)
    {
        Replace(start, length, new string('\n', Span.Slice(start, length).Count('\n')));
    }
}