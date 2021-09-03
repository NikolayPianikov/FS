namespace FS.Core
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class StreamWriter: IWriter
    {
        private readonly Stream _stream;
        private readonly SemaphoreSlim _syncSemaphore = SemaphoreSlimExtensions.CreateSyncSemaphore();

        public StreamWriter(Stream stream) => _stream = stream;

        public int Write(ReadOnlySpan<byte> source, long destinationPosition)
        {
            using var syncToken = _syncSemaphore.CreateSyncToken();
            _stream.Position = destinationPosition;
            _stream.Write(source);
            return source.Length;
        }
        
        public async ValueTask<int> WriteAsync(ReadOnlyMemory<byte> source, long destinationPosition)
        {
            using var syncToken = await _syncSemaphore.CreateSyncTokenAsync();
            _stream.Position = destinationPosition;
            await _stream.WriteAsync(source);
            return source.Length;
        }

        public void Flush()
        {
            using var syncToken = _syncSemaphore.CreateSyncToken();
            _stream.Flush();
        }

        public async Task FlushAsync()
        {
            using var syncToken = await _syncSemaphore.CreateSyncTokenAsync();
            await _stream.FlushAsync();
        }

        public void Dispose()
        {
            using (_syncSemaphore)
            using (_syncSemaphore.CreateSyncToken())
            {
                _stream.Dispose();
            }
        }
    }
}