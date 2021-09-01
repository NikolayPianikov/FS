namespace FS.Core
{
    using System;

    internal sealed class RawFileSystem : IRawFileSystem
    {
        private readonly IBlockAllocationTable _table;
        private readonly IChainSplitter _splitter;
        private readonly IWriter? _writer;
        private readonly IReader? _reader;

        public RawFileSystem(IBlockAllocationTable table, IChainSplitter splitter, RawFileSystemSettings settings)
        {
            _table = table;
            _splitter = splitter;
            _writer = settings.Writer;
            _reader = settings.Reader;
        }
        
        public bool TryCreateFile(out RawFile file)
        {
            if (_table.CreateBlockChain(out var firstBlock))
            {
                file = new RawFile(firstBlock);
                return true;
            }

            file = default;
            return false;
        }

        public void DeleteFile(RawFile file)
        {
            var firstBlock = file.FirstBlock;
            _table.ReleaseBlockChain(firstBlock);
        }
        
        public IReader CreateReader(RawFile file) => new BlockReader(file.FirstBlock, _splitter, GuardInitialized(_reader));

        public IWriter CreateWriter(RawFile file) => new BlockWriter(file.FirstBlock, _table, _splitter, GuardInitialized(_writer));

        private static T GuardInitialized<T>(T? instance)
            where T: class =>
            instance ?? throw new InvalidOperationException("Not initialized");
    }
}