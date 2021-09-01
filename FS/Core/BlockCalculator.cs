// ReSharper disable BuiltInTypeReferenceStyle
namespace FS.Core
{
    using System;
    using System.Runtime.InteropServices;

    internal sealed class BlockCalculator : IBlockCalculator, ISettings<IBlockCalculator, BlockCalculatorSettings>
    {
        public const int DefaultBlockSize = 8192;
        public const int DefaultBlockCount = 4096; 
        private int _headerSize;

        public BlockCalculator(int blockSize = DefaultBlockSize, int blockCount = DefaultBlockCount) => Setup(blockSize, blockCount);

        public IBlockCalculator Apply(BlockCalculatorSettings settings) => Setup(settings.BlockSize, settings.BlockCount);

        private IBlockCalculator Setup(int blockSize, int blockCount)
        {
            if (blockSize <= 0) throw new ArgumentOutOfRangeException(nameof(blockSize));
            if (blockCount <= 0) throw new ArgumentOutOfRangeException(nameof(blockCount));
            checked
            {
                BlockSize = blockSize;
                BlockCount = blockCount;
                _headerSize = blockCount * Marshal.SizeOf<Block>();
                var dataSize = blockCount * blockSize;
                SectorSize = _headerSize + dataSize;
            }

            return this;
        }

        public int BlockSize { get; private set; }

        public int BlockCount { get; private set; }
        
        public int SectorSize { get; private set; }
        
        public long BlockToPosition(Block block)
        {
            if (block.BlockId > BlockCount) throw new ArgumentOutOfRangeException(nameof(block.BlockId));
            checked
            {
                return SectorToPosition(block.SectorId) + _headerSize + block.BlockId * BlockSize;
            }
        }

        public long SectorToPosition(int sectorId)
        {
            checked
            {
                return (long)sectorId * SectorSize;
            }
        }
    }
}