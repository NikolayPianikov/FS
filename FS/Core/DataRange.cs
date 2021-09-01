namespace FS.Core
{
    using System;

    internal readonly struct DataRange
    {
        public static readonly DataRange Uncompleted = new(0);
        public readonly long Position;
        public readonly int Size;

        public DataRange(long position, int size)
        {
#if DEBUG
            if (position < 0) throw new ArgumentOutOfRangeException(nameof(position));
            if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));
#endif
            Position = position;
            Size = size;
        }
        
        private DataRange(int flag)
        {
            Position = long.MinValue;
            Size = flag;
        }

        public override bool Equals(object? obj) => obj is DataRange other && Position == other.Position && Size == other.Size;

        public override int GetHashCode() => HashCode.Combine(Position, Size);

        public override string ToString() => $"{Position}: {Size}";
    }
}