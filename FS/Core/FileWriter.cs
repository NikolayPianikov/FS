namespace FS.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    internal sealed class FileWriter: IWriter
    {
        private readonly FileStream _stream;

        public FileWriter(FileStream stream) =>
            _stream = stream;

        public int Write(ReadOnlySpan<byte> source, long destinationPosition)
        {
            _stream.Position = destinationPosition;
            _stream.Write(source);
            return source.Length;
        }
        
        public async ValueTask<int> WriteAsync(ReadOnlyMemory<byte> source, long destinationPosition)
        {
            _stream.Position = destinationPosition;
            await _stream.WriteAsync(source);
            return source.Length;
        }
    }
}