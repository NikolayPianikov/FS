namespace FS.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    internal sealed class FileReader : IReader
    {
        private readonly FileStream _stream;

        public FileReader(FileStream stream) =>
            _stream = stream;

        public int Read(long sourcePosition, Span<byte> destination)
        {
            _stream.Position = sourcePosition;
            return _stream.Read(destination);
        }

        public ValueTask<int> ReadAsync(long sourcePosition, Memory<byte> destination)
        {
            _stream.Position = sourcePosition;
            return _stream.ReadAsync(destination);
        }
    }
}