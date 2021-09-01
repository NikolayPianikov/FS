namespace FS
{
    using Core;

    public readonly struct RawFileSystemSettings
    {
        public readonly IReader Reader;
        public readonly IWriter Writer;
        public readonly int BlockSize;
        public readonly int BlockCount;

        public RawFileSystemSettings(
            IReader reader,
            IWriter writer,
            int blockSize = BlockCalculator.DefaultBlockSize,
            int blockCount = BlockCalculator.DefaultBlockCount)
        {
            Reader = reader;
            Writer = writer;
            BlockSize = blockSize;
            BlockCount = blockCount;
        }
    }
}