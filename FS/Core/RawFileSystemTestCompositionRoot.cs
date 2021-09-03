// ReSharper disable IdentifierTypo
namespace FS.Core
{
    // For tests
    internal readonly struct RawFileSystemTestCompositionRoot<T>
    {
        public readonly ISettings<IRawFileSystem, RawFileSystemSettings<T>> Settings;
        public readonly IRawFileSystem FileSystem;
        public readonly IBlockAllocationTableStatistics TableStatistics;
        public readonly IRawFileSystemStatistics FileSystemStatistics;

        public RawFileSystemTestCompositionRoot(
            ISettings<IRawFileSystem, RawFileSystemSettings<T>> settings,
            IRawFileSystem fileSystem,
            IBlockAllocationTableStatistics tableStatistics,
            IRawFileSystemStatistics fileSystemStatistics)
        {
            Settings = settings;
            FileSystem = fileSystem;
            TableStatistics = tableStatistics;
            FileSystemStatistics = fileSystemStatistics;
        }
    }
}