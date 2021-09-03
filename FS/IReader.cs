namespace FS
{
    using System;
    using System.Threading.Tasks;

    public interface IReader: IDisposable
    {
        int Read(long sourcePosition, Span<byte> destination);

        ValueTask<int> ReadAsync(long sourcePosition, Memory<byte> destination);
    }
}