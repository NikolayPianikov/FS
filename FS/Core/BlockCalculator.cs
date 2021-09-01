// ReSharper disable BuiltInTypeReferenceStyle
namespace FS.Core
{
    using System;
    using System.Runtime.InteropServices;

    internal sealed class BlockCalculator : IBlockCalculator
    {
        private readonly int _headerSize;

        public BlockCalculator(RawFileSystemSettings settings)
            :this(settings.BlockSize, settings.BlockCount)
        { }

        public BlockCalculator(int blockSize, int blockCount)
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
        }

        public int BlockSize { get; }
        
        public int BlockCount { get; }
        
        public int SectorSize { get; }
        
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