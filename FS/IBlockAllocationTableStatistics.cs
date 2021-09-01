namespace FS
{
    public interface IBlockAllocationTableStatistics
    {
        int NumberOfSectorsUsed { get; }

        int NumberOfBlocksUsed { get; }
    }
}