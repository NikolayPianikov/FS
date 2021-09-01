// ReSharper disable ClassNeverInstantiated.Global
namespace FS.Core
{
    using System.Buffers;

    internal sealed class MemoryManager<T> : IMemoryManager<T>
    {
        private readonly MemoryPool<T> _pool;

        public MemoryManager(MemoryPool<T> pool) => _pool = pool;

        public IMemoryOwner<T> Rent(int minBufferSize = -1) => _pool.Rent(minBufferSize);
    }
}