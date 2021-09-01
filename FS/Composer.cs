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
                .Bind<IContext<TTS>>().To<Context<TTS>>()
                .Bind<IFactory<RawFileSystemSettings, IRawFileSystem>>().To<RawFileSystemFactory>()
            .Default(PerResolve)
                .Bind<RawFileSystemSettings>().To(ctx => ctx.Resolve<IContext<RawFileSystemSettings>>().Data)
                .Bind<IBlockCalculator>().To<BlockCalculator>()
                .Bind<IBlockAllocationTable>().Tag("base").To<BlockAllocationTable>()
                .Bind<IBlockAllocationTable>().To<BlockAllocationTableSafe>()
                .Bind<IRawFileSystem>().To<RawFileSystem>()
                .Bind<IChainSplitter>().To<ChainSplitter>()
                .Bind<IRawFileSystem>().To<RawFileSystem>();
    }
}