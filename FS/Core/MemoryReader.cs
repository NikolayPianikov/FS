namespace FS.Core
{
    using System;
    using System.Threading.Tasks;

    internal sealed class MemoryReader: IReader
    {
        private readonly Memory<byte> _memory;

        public MemoryReader(Memory<byte> memory) => _memory = memory;

        public int Read(long sourcePosition, Span<byte> destination)
        {
            var size = (int)(_memory.Length >= destination.Length + sourcePosition ? destination.Length : _memory.Length - sourcePosition);
            if (size <= 0)
            {
                return 0;
            }
            
            _memory.Slice((int)sourcePosition, size).Span.CopyTo(destination);
            return size;
        }

        public ValueTask<int> ReadAsync(long sourcePosition, Memory<byte> destination) =>
            new(Read(sourcePosition, destination.Span));
    }
}