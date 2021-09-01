// ReSharper disable BuiltInTypeReferenceStyle
namespace FS.Core
{
    using Position = System.Int64;

    internal interface IBlockCalculator
    {
        int BlockSize { get; }
        
        int BlockCount { get; }

        int SectorSize { get; }

        Position BlockToPosition(Block block);
        
        Position SectorToPosition(int sectorId);
    }
}