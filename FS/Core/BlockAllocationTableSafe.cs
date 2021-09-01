namespace FS.Core
{
    using Pure.DI;

    internal sealed class BlockAllocationTableSafe: IBlockAllocationTable
    {
        private readonly IBlockAllocationTable _table;
        private readonly object _lockObject = new();

        public BlockAllocationTableSafe([Tag("base")] IBlockAllocationTable table) =>
            _table = table;

        public void Dispose()
        {
            lock (_lockObject)
            {
                _table.Dispose();
            }
        }

        public bool CreateBlockChain(out Block firstBlock)
        {
            lock (_lockObject)
            {
                return _table.CreateBlockChain(out firstBlock);
            }
        }

        public bool TryGetNextBlockInChain(Block currentBlock, out Block nextBlock, bool expansion)
        {
            lock (_lockObject)
            {
                return _table.TryGetNextBlockInChain(currentBlock, out nextBlock, expansion);
            }
        }

        public void ReleaseBlockChain(Block firstBlock)
        {
            lock (_lockObject)
            {
                _table.ReleaseBlockChain(firstBlock);
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
    }
}