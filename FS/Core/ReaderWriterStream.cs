// ReSharper disable UnusedMember.Global
namespace FS.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    internal sealed class ReaderWriterStream : Stream
    {
        private readonly IReader? _reader;
        private readonly IWriter? _writer;

        public ReaderWriterStream(IReader? reader, IWriter? writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public override bool CanRead => _reader != null;

        public override bool CanSeek => false;

        public override bool CanWrite => _writer != null;

        public override long Length => throw new NotImplementedException();

        public override long Position { get; set; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytes = (_reader ?? throw new InvalidOperationException("Cannot read.")).Read(Position, new Span<byte>(buffer, offset, count));
            Position += bytes;
            return bytes;
        }

        public int Read(long sourcePosition, Span<byte> destination)
        {
            var bytes = (_reader ?? throw new InvalidOperationException("Cannot read.")).Read(sourcePosition, destination);
            Position += bytes;
            return bytes;
        }

        public async ValueTask<int> ReadAsync(long sourcePosition, Memory<byte> destination)
        {
            var bytes = await (_reader ?? throw new InvalidOperationException("Cannot read.")).ReadAsync(sourcePosition, destination);
            Position += bytes;
            return bytes;
        }

        public override void Write(byte[] buffer, int offset, int count) =>
            (_writer ?? throw new InvalidOperationException("Cannot write.")).Write(new Span<byte>(buffer, offset, count), Position);

        public override void Flush() => _writer?.Flush();

        protected override void Dispose(bool disposing)
        {
            _reader?.Dispose();
            _writer?.Dispose();
            base.Dispose(disposing);
        }

        public override ValueTask DisposeAsync()
        {
            _reader?.Dispose();
            _writer?.Dispose();
            return base.DisposeAsync();
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

        public override void SetLength(long value) => throw new NotImplementedException();
    }
}