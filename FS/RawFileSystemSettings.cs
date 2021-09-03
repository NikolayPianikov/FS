namespace FS
{
    using Core;

    public readonly struct RawFileSystemSettings<T>
    {
        public readonly T File;
        public readonly int BlockSize;
        public readonly int BlockCount;

        public RawFileSystemSettings(
            T file,
            int blockSize = BlockCalculator.DefaultBlockSize,
            int blockCount = BlockCalculator.DefaultBlockCount)
        {
            File = file;
            BlockSize = blockSize;
            BlockCount = blockCount;
        }
    }
}