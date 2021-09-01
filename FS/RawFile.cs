namespace FS
{
    public readonly struct RawFile
    {
        public readonly Block FirstBlock;

        public RawFile(Block firstBlock) => FirstBlock = firstBlock;

        public override string ToString() => FirstBlock.ToString();

        public override bool Equals(object? obj) => obj is RawFile other && FirstBlock.Equals(other.FirstBlock);

        public override int GetHashCode() => FirstBlock.GetHashCode();
    }
}