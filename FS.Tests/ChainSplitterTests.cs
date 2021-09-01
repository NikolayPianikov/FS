namespace FS.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Shouldly;
    using Xunit;

    public class ChainSplitterTests
    {
        private const int BlockSize = 2;
        private const int BlockCount = 4;
        private readonly IBlockCalculator _calculator = new BlockCalculator(BlockSize, BlockCount);
        
        [Fact]
        public void ShouldSplit()
        {
            // Given
            var firstBlock = new Block(0, 0);
            var nextBlock1 = new Block(0, 1);
            var nextBlock2 = new Block(1, 2);

            var table = new TableStub(firstBlock, nextBlock1, nextBlock2);
            var splitter = CreateInstance(table);

            // When
            var actualBlocks = splitter.Split(firstBlock, 1, 4, false).ToArray();

            // Then
            actualBlocks.ShouldBe(new []
            {
                new DataRange(_calculator.BlockToPosition(firstBlock) + 1, 1),
                new DataRange(_calculator.BlockToPosition(nextBlock1), 2),
                new DataRange(_calculator.BlockToPosition(nextBlock2), 1)
            });
        }
        
        [Fact]
        public void ShouldSplitWhenOneBlock()
        {
            // Given
            var firstBlock = new Block(0, 0);
            
            var table = new TableStub();
            var splitter = CreateInstance(table);

            // When
            var actualBlocks = splitter.Split(firstBlock, 1, 1, false).ToArray();

            // Then
            actualBlocks.ShouldBe(new [] { new DataRange(_calculator.BlockToPosition(firstBlock) + 1, 1) });
        }
        
        [Fact]
        public void ShouldSplitWhenNotEnoughBlocks()
        {
            // Given
            var firstBlock = new Block(0, 0);
            var nextBlock1 = new Block(0, 1);

            var table = new TableStub(firstBlock, nextBlock1);
            var splitter = CreateInstance(table);

            // When
            var actualBlocks = splitter.Split(firstBlock, 1, 4, false).ToArray();

            // Then
            actualBlocks.ShouldBe(new []
            {
                new DataRange(_calculator.BlockToPosition(firstBlock) + 1, 1),
                new DataRange(_calculator.BlockToPosition(nextBlock1), 2),
                DataRange.Uncompleted
            });
        }
        
        private ChainSplitter CreateInstance(IBlockAllocationTable table) =>
            new(_calculator, table);
        
        private class TableStub: IBlockAllocationTable
        {
            private readonly Dictionary<Block, Block> _chain = new();
            
            public TableStub(params Block[] chain)
            {
                for (var i = 0; i < chain.Length - 1; i++)
                {
                    _chain[chain[i]] = chain[i + 1];
                }
            }

            public bool TryGetNextBlockInChain(Block currentBlock, out Block nextBlock, bool expansion) => _chain.TryGetValue(currentBlock, out nextBlock);

            public void Dispose() => throw new System.NotImplementedException();

            public bool CreateBlockChain(out Block firstBlock) => throw new System.NotImplementedException();

            public void ReleaseBlockChain(Block firstBlock) => throw new System.NotImplementedException();

            public bool TrySave(IWriter writer) => throw new System.NotImplementedException();

            public bool TryLoad(IReader reader) => throw new System.NotImplementedException();
        }
    }
}