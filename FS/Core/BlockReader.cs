// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ClassNeverInstantiated.Global
namespace FS.Core
{
    using System;
    using System.Threading.Tasks;

    internal sealed class BlockReader: IReader
    {
        private readonly Block _firstBlock;
        private readonly IChainSplitter _splitter;
        private readonly IReader _reader;

        public BlockReader(Block firstBlock, IChainSplitter splitter, IReader reader)
        {
            _firstBlock = firstBlock;
            _splitter = splitter;
            _reader = reader;
        }

        public int Read(long sourcePosition, Span<byte> destination)
        {
            var position = 0;
            foreach (var dataBlock in _splitter.Split(_firstBlock, sourcePosition, destination.Length, false))
            {
                if (dataBlock.Equals(DataRange.Uncompleted))
                {
                    return position;
                }
                
                position += _reader.Read(dataBlock.Position, destination.Slice(position, dataBlock.Size));
            }

            return position;
        }

        public async ValueTask<int> ReadAsync(long sourcePosition, Memory<byte> destination)
        {
            var position = 0;
            foreach (var dataBlock in _splitter.Split(_firstBlock, sourcePosition, destination.Length, false))
            {
                if (dataBlock.Equals(DataRange.Uncompleted))
                {
                    return position;
                }

                position += await _reader.ReadAsync(dataBlock.Position, destination.Slice(position, dataBlock.Size));
            }

            return position;
        }
    }
}