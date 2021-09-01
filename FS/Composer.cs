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
                .Bind<IReader>().Bind<IWriter>().Tag(WellknownTag.NotInitialized).To<NullReaderWriter>()
            .Default(PerResolve)
                .Bind<IBlockCalculator>().Bind<ISettings<IBlockCalculator, BlockCalculatorSettings>>().To<BlockCalculator>()
                .Bind<IBlockAllocationTable>().Bind<IBlockAllocationTableStatistics>().Tag(WellknownTag.Base).To<BlockAllocationTable>()
                .Bind<IBlockAllocationTable>().Bind<IBlockAllocationTableStatistics>().To<BlockAllocationTableSafe>()
                .Bind<IRawFileSystem>().To<RawFileSystem>()
                .Bind<IChainSplitter>().To<ChainSplitter>()
                .Bind<RawFileSystemTestCompositionRoot>().To<RawFileSystemTestCompositionRoot>() // For tests
                .Bind<IRawFileSystem>().Bind<ISettings<IRawFileSystem, RawFileSystemSettings>>().Bind<ICache>().Bind<IRawFileSystemStatistics>().To<RawFileSystem>();
    }
}