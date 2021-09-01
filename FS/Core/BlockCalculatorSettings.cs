namespace FS.Core
{
    internal readonly struct BlockCalculatorSettings
    {
        public readonly int BlockSize;
        public readonly int BlockCount;

        public BlockCalculatorSettings(int blockSize, int blockCount)
        {
            BlockSize = blockSize;
            BlockCount = blockCount;
        }
    }
}