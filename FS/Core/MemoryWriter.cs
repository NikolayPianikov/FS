namespace FS.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    internal sealed class MemoryWriter: IWriter
    {
        private readonly Memory<byte> _memory;

        public MemoryWriter(Memory<byte> memory) => _memory = memory;

        public int Write(ReadOnlySpan<byte> source, long destinationPosition)
        {
            if (source.Length + destinationPosition > _memory.Length)
            {
                throw new IOException();
            }

            source.CopyTo(_memory.Slice((int)destinationPosition, source.Length).Span);
            return source.Length;
        }

        public ValueTask<int> WriteAsync(ReadOnlyMemory<byte> source, long destinationPosition) =>
            new(Write(source.Span, destinationPosition));

        public bool TryFlush() => true;

        public Task<bool> TryFlushAsync() => Task.FromResult(true);
    }
}