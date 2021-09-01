// ReSharper disable BuiltInTypeReferenceStyle
namespace FS.Core
{
    internal interface IBlockCalculator
    {
        int BlockSize { get; }
        
        int BlockCount { get; }

        int SectorSize { get; }

        long BlockToPosition(Block block);
        
        long SectorToPosition(int sectorId);
    }
}