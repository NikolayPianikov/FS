namespace FS
{
    using System;

    public readonly struct Block: IEquatable<Block>
    {
        public static readonly Block Empty = new(0);
        public static readonly Block Last = new(1);
        public readonly int SectorId;
        public readonly int BlockId;

        public Block(int sectorId, int blockId)
        {
#if DEBUG
            if (sectorId < 0) throw new ArgumentOutOfRangeException(nameof(sectorId));
            if (blockId < 0) throw new ArgumentOutOfRangeException(nameof(blockId));
#endif
            SectorId = sectorId;
            BlockId = blockId;
        }
        
        private Block(int flag)
        {
            SectorId = int.MinValue;
            BlockId = flag;
        }

        public override string ToString() => $"{SectorId}: {BlockId}";

        public bool Equals(Block other) => SectorId == other.SectorId && BlockId == other.BlockId;

        public override bool Equals(object? obj) => obj is Block other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(SectorId, BlockId);
    }
}