namespace FS
{
    public readonly struct RawFileSystemSettings
    {
        public readonly IReader Reader;
        public readonly IWriter Writer;
        public readonly int BlockSize;
        public readonly int BlockCount;

        public RawFileSystemSettings(
            IReader reader,
            IWriter writer,
            int blockSize = 8192,
            int blockCount = 4096)
        {
            Reader = reader;
            Writer = writer;
            BlockSize = blockSize;
            BlockCount = blockCount;
        }
    }
}