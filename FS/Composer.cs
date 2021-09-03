namespace FS
{
    using System.Buffers;
    using Core;
    using Pure.DI;
    using static Pure.DI.Lifetime;

    internal static partial class Composer
    {
        static Composer() => DI.Setup()
            // Raw file system
            .Default(Singleton)
                .Bind<MemoryPool<TT>>().To(_ => MemoryPool<TT>.Shared)
                .Bind<IMemoryManager<TT>>().To<Core.MemoryManager<TT>>()
                .Bind<IReaderWriterFactory<string>>().To<FileReaderWriterFactory>()
            .Default(PerResolve)
                .Bind<IBlockCalculator>().Bind<ISettings<IBlockCalculator, BlockCalculatorSettings>>().To<BlockCalculator>()
                .Bind<IBlockAllocationTable>().Bind<IBlockAllocationTableStatistics>().Tag(WellknownTag.Base).To<BlockAllocationTable>()
                .Bind<IBlockAllocationTable>().Bind<IBlockAllocationTableStatistics>().To<BlockAllocationTableSafe>()
                .Bind<IChainSplitter>().To<ChainSplitter>()
                .Bind<RawFileSystemTestCompositionRoot<string>>().To<RawFileSystemTestCompositionRoot<string>>() // For tests
                .Bind<string>().Tag(WellknownTag.Default).To(_ => string.Empty)
                .Bind<IRawFileSystem>().Bind<ISettings<IRawFileSystem, RawFileSystemSettings<string>>>().Bind<IRawFileSystemStatistics>().To<RawFileSystem<string>>();
    }
}