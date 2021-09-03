namespace FS.Core
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class StreamReader : IReader
    {
        private readonly Stream _stream;
        private readonly SemaphoreSlim _semaphore = SemaphoreSlimExtensions.CreateSyncSemaphore();

        public StreamReader(Stream stream) => _stream = stream;

        public int Read(long sourcePosition, Span<byte> destination)
        {
            using var syncToken = _semaphore.CreateSyncToken();
            _stream.Position = sourcePosition;
            return _stream.Read(destination);
        }

        public async ValueTask<int> ReadAsync(long sourcePosition, Memory<byte> destination)
        {
            using var syncToken = await _semaphore.CreateSyncTokenAsync();
            _stream.Position = sourcePosition;
            return await _stream.ReadAsync(destination);
        }

        public void Dispose()
        {
            using (_semaphore)
            using (_semaphore.CreateSyncToken())
            {
                _stream.Dispose();
            }
        }
    }
}