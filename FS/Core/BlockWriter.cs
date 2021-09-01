namespace FS.Core
{
    using System;
    using System.Threading.Tasks;

    internal sealed class BlockWriter: IWriter
    {
        private readonly Block _firstBlock;
        private readonly IChainSplitter _splitter;
        private readonly IWriter _writer;
        
        public BlockWriter(Block firstBlock, IChainSplitter splitter, IWriter writer)
        {
            _firstBlock = firstBlock;
            _splitter = splitter;
            _writer = writer;
        }

        public int Write(ReadOnlySpan<byte> source, long destinationPosition)
        {
            var position = 0;
            foreach (var dataBlock in _splitter.Split(_firstBlock, destinationPosition, source.Length, true))
            {
                if (dataBlock.Equals(DataRange.Uncompleted))
                {
                    return position;
                }
                
                _writer.Write(source.Slice(position, dataBlock.Size), dataBlock.Position);
                position += dataBlock.Size;
            }

            return position;
        }

        public async ValueTask<int> WriteAsync(ReadOnlyMemory<byte> source, long destinationPosition)
        {
            var position = 0;
            foreach (var dataBlock in _splitter.Split(_firstBlock, destinationPosition, source.Length, true))
            {
                if (dataBlock.Equals(DataRange.Uncompleted))
                {
                    return position;
                }
                
                await _writer.WriteAsync(source.Slice(position, dataBlock.Size), dataBlock.Position);
                position += dataBlock.Size;
            }

            return position;
        }
    }
}