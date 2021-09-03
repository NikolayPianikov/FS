// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable IdentifierTypo
namespace FS.Core
{
    using System.Threading;
    using Pure.DI;

    internal sealed class RawFileSystem<T> :
        IRawFileSystem,
        ISettings<IRawFileSystem, RawFileSystemSettings<T>>,
        IRawFileSystemStatistics
    {
        private readonly ISettings<IBlockCalculator, BlockCalculatorSettings> _calculatorSettings;
        private readonly IBlockAllocationTable _table;
        private readonly IChainSplitter _splitter;
        private readonly IReaderWriterFactory<T> _readerWriterFactory;
        private T _file;
        private volatile int _numberOfFiles;

        public RawFileSystem(
            [Tag(WellknownTag.Default)] T defaultFile,
            ISettings<IBlockCalculator, BlockCalculatorSettings> calculatorSettings,
            IBlockAllocationTable table,
            IChainSplitter splitter,
            IReaderWriterFactory<T> readerWriterFactory)
        {
            _file = defaultFile;
            _calculatorSettings = calculatorSettings;
            _table = table;
            _splitter = splitter;
            _readerWriterFactory = readerWriterFactory;
        }

        public int NumberOfFiles => _numberOfFiles;
        
        public IRawFileSystem Apply(RawFileSystemSettings<T> settings)
        {
            _file = settings.File;
            _calculatorSettings.Apply(new BlockCalculatorSettings(settings.BlockSize, settings.BlockCount));
            using var reader = _readerWriterFactory.CreateReader(_file);
            _table.TryLoad(reader);
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
        
        public IReader CreateReader(RawFile file) => new BlockReader(file.FirstBlock, _splitter, _readerWriterFactory.CreateReader(_file));

        public IWriter CreateWriter(RawFile file) => new BlockWriter(file.FirstBlock, _splitter, _readerWriterFactory.CreateWriter(_file));

        public bool TryFlush()
        {
            using var writer = _readerWriterFactory.CreateWriter(_file);
            try
            {
                return _table.TrySave(writer);
            }
            finally
            {
                writer.Flush();
            }
        }
    }
}