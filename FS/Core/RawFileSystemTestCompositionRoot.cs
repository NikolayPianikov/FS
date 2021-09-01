// ReSharper disable IdentifierTypo
namespace FS.Core
{
    // For tests
    internal readonly struct RawFileSystemTestCompositionRoot
    {
        public readonly ISettings<IRawFileSystem, RawFileSystemSettings> Settings;
        public readonly IRawFileSystem FileSystem;
        public readonly ICache Cache;
        public readonly IBlockAllocationTableStatistics TableStatistics;
        public readonly IRawFileSystemStatistics FileSystemStatistics;

        public RawFileSystemTestCompositionRoot(
            ISettings<IRawFileSystem, RawFileSystemSettings> settings,
            IRawFileSystem fileSystem,
            ICache cache,
            IBlockAllocationTableStatistics tableStatistics,
            IRawFileSystemStatistics fileSystemStatistics)
        {
            Settings = settings;
            FileSystem = fileSystem;
            Cache = cache;
            TableStatistics = tableStatistics;
            FileSystemStatistics = fileSystemStatistics;
        }
    }
}