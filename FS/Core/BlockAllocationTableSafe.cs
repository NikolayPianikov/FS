namespace FS.Core
{
    using Pure.DI;

    internal sealed class BlockAllocationTableSafe: IBlockAllocationTable, IBlockAllocationTableStatistics
    {
        private readonly IBlockAllocationTable _table;
        private readonly IBlockAllocationTableStatistics _statistics;
        private readonly object _lockObject = new();

        public BlockAllocationTableSafe(
            [Tag(WellknownTag.Base)] IBlockAllocationTable table,
            [Tag(WellknownTag.Base)] IBlockAllocationTableStatistics statistics)
        {
            _table = table;
            _statistics = statistics;
        }

        public void Dispose()
        {
            lock (_lockObject)
            {
                _table.Dispose();
            }
        }

        public bool TryCreateBlockChain(out Block firstBlock)
        {
            lock (_lockObject)
            {
                return _table.TryCreateBlockChain(out firstBlock);
            }
        }

        public bool TryGetNextBlockInChain(Block currentBlock, out Block nextBlock, bool expansion)
        {
            lock (_lockObject)
            {
                return _table.TryGetNextBlockInChain(currentBlock, out nextBlock, expansion);
            }
        }

        public bool TryReleaseBlockChain(Block firstBlock)
        {
            lock (_lockObject)
            {
                return _table.TryReleaseBlockChain(firstBlock);
            }
        }

        public bool TrySave(IWriter writer)
        {
            lock (_lockObject)
            {
                return _table.TrySave(writer);
            }
        }

        public bool TryLoad(IReader reader)
        {
            lock (_lockObject)
            {
                return _table.TryLoad(reader);
            }
        }

        public int NumberOfSectorsUsed
        {
            get
            {
                lock (_lockObject)
                {
                    return _statistics.NumberOfSectorsUsed;
                }
            }
        }

        public int NumberOfBlocksUsed
        {
            get
            {
                lock (_lockObject)
                {
                    return _statistics.NumberOfBlocksUsed;
                }
            }
        }
    }
}