namespace FS
{
    using System;
    using System.Threading.Tasks;

    public interface IWriter
    {
        int Write(ReadOnlySpan<byte> source, long destinationPosition);
        
        ValueTask<int> WriteAsync(ReadOnlyMemory<byte> source, long destinationPosition);
    }
}