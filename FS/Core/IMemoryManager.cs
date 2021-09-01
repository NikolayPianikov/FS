namespace FS.Core
{
    using System.Buffers;

    internal interface IMemoryManager<T>
    {
        IMemoryOwner<T> Rent(int minBufferSize = -1);
    }
}