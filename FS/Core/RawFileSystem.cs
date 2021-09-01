// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable IdentifierTypo
namespace FS.Core
{
    using System.Threading;
    using Pure.DI;

    internal sealed class RawFileSystem :
        IRawFileSystem,
        ISettings<IRawFileSystem, RawFileSystemSettings>,
        ICache,
        IRawFileSystemStatistics
    {
        private readonly ISettings<IBlockCalculator, BlockCalculatorSettings> _calculatorSettings;
        private readonly IBlockAllocationTable _table;
        private readonly IChainSplitter _splitter;
        private IWriter _writer;
        private IReader _reader;
        private volatile int _numberOfFiles;
        
        public RawFileSystem(
            ISettings<IBlockCalculator, BlockCalculatorSettings> calculatorSettings,
            IBlockAllocationTable table,
            IBlockAllocationTableStatistics statistics,
            IChainSplitter splitter,
            [Tag(WellknownTag.NotInitialized)] IReader reader,
            [Tag(WellknownTag.NotInitialized)] IWriter writer)
        {
            _calculatorSettings = calculatorSettings;
            _table = table;
            _splitter = splitter;
            _reader = reader;
            _writer = writer;
        }

        public int NumberOfFiles => _numberOfFiles;
        
        public IRawFileSystem Apply(RawFileSystemSettings settings)
        {
            _calculatorSettings.Apply(new BlockCalculatorSettings(settings.BlockSize, settings.BlockCount));
            _writer = settings.Writer;
            _reader = settings.Reader;
            _table.TryLoad(_reader);
            return this;
        }

        public bool TryCreateFile(out RawFile file)
        {
            if (_table.TryCreateBlockChain(out var firstBlock))
            {
                Interlocked.Increment(ref _numberOfFiles);
                file = new RawFile(firstBlock);
                return true;
            }

            file = default;
            return false;
        }

        public bool TryDeleteFile(RawFile file)
        {
            var firstBlock = file.FirstBlock;
            // ReSharper disable once InvertIf
            if (_table.TryReleaseBlockChain(firstBlock))
            {
                Interlocked.Decrement(ref _numberOfFiles);
                return true;
            }

            return false;
        }
        
        public IReader CreateReader(RawFile file) => new BlockReader(file.FirstBlock, _splitter, _reader);

        public IWriter CreateWriter(RawFile file) => new BlockWriter(file.FirstBlock, _splitter, _writer);

        public bool TryFlush() => _table.TrySave(_writer);
    }
}