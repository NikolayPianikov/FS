namespace FS.Core
{
    using System;
    using System.Collections.Generic;

    internal sealed class ChainSplitter : IChainSplitter
    {
        private readonly IBlockCalculator _calculator;
        private readonly IBlockAllocationTable _table;

        public ChainSplitter(IBlockCalculator calculator, IBlockAllocationTable table)
        {
            _calculator = calculator;
            _table = table;
        }

        public IEnumerable<DataRange> Split(Block firstBlock, long position, long size, bool expansion)
        {
            if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));
            if (position < 0) throw new ArgumentOutOfRangeException(nameof(position));
            if (size == 0)
            {
                yield break;
            }

            var blockSize = _calculator.BlockSize;
            var skipBlocks = position / blockSize;
            var currentBlock = firstBlock;

            // Skip blocks to position
            while (skipBlocks > 0)
            {
                if (!_table.TryGetNextBlockInChain(currentBlock, out currentBlock, expansion))
                {
                    yield break;
                }

                skipBlocks--;
            }

            var dataOffset = (int)(position % blockSize);
            var dataSize = blockSize - dataOffset;
            do
            {
                if (dataSize > size)
                {
                    dataSize = (int)size;
                }
                
                yield return new DataRange(_calculator.BlockToPosition(currentBlock) + dataOffset, dataSize);
                size -= dataSize;
                dataSize = blockSize;
                dataOffset = 0;
            } while (size > 0 && _table.TryGetNextBlockInChain(currentBlock, out currentBlock, expansion));

            if (size > 0)
            {
                yield return DataRange.Uncompleted;
            }
        }
    }
}