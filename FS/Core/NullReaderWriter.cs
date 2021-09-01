namespace FS.Core
{
    using System;
    using System.Threading.Tasks;

    internal class NullReaderWriter: IReader, IWriter
    {
        private static readonly Exception NotConfiguredException = new InvalidOperationException($"Not configured. Run method {nameof(ISettings<IRawFileSystem, RawFileSystemSettings>.Apply)} of interface {nameof(ISettings<IRawFileSystem, RawFileSystemSettings>)} before.");
        
        public int Read(long sourcePosition, Span<byte> destination) => throw NotConfiguredException;

        public ValueTask<int> ReadAsync(long sourcePosition, Memory<byte> destination) => throw NotConfiguredException;
        
        public int Write(ReadOnlySpan<byte> source, long destinationPosition) => throw NotConfiguredException;

        public ValueTask<int> WriteAsync(ReadOnlyMemory<byte> source, long destinationPosition) => throw NotConfiguredException;
    }
}