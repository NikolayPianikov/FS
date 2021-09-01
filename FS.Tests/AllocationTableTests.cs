// ReSharper disable BuiltInTypeReferenceStyle
namespace FS.Tests
{
    using System.Buffers;
    using Core;
    using Shouldly;
    using Xunit;
    using Position = System.Int64;

    public class AllocationTableTests: IMemoryManager<Block>
    {
        private const int BlockSize = 2;
        private const int BlockCount = 4;
        private readonly IBlockCalculator _calculator = new BlockCalculator(BlockSize, BlockCount);

        public IMemoryOwner<Block> Rent(int minBufferSize = -1) => MemoryPool<Block>.Shared.Rent(minBufferSize);

        [Fact]
        public void ShouldCreateBlockChain()
        {
            // Given
            using var table = CreateInstance();

            // When
            table.TryCreateBlockChain(out var firstBlock0).ShouldBeTrue();
            table.TryCreateBlockChain(out var firstBlock1).ShouldBeTrue();
            table.TryCreateBlockChain(out _).ShouldBeTrue();
            table.TryCreateBlockChain(out _).ShouldBeTrue();
            table.TryCreateBlockChain(out var firstBlock4).ShouldBeTrue();
            table.TryCreateBlockChain(out var firstBlock5).ShouldBeTrue();

            // Then
            firstBlock0.SectorId.ShouldBe(0);
            firstBlock0.BlockId.ShouldBe(0);
            table.GetNextBlock(ref firstBlock0).ShouldBe(Block.Last);
            
            firstBlock1.SectorId.ShouldBe(0);
            firstBlock1.BlockId.ShouldBe(1);
            table.GetNextBlock(ref firstBlock1).ShouldBe(Block.Last);
            
            firstBlock4.SectorId.ShouldBe(1);
            firstBlock4.BlockId.ShouldBe(0);
            table.GetNextBlock(ref firstBlock4).ShouldBe(Block.Last);
            
            firstBlock5.SectorId.ShouldBe(1);
            firstBlock5.BlockId.ShouldBe(1);
            table.GetNextBlock(ref firstBlock5).ShouldBe(Block.Last);
            
            table.NumberOfSectorsUsed.ShouldBe(2);
            table.NumberOfBlocksUsed.ShouldBe(6);
        }
        
        [Fact]
        public void ShouldGetNextBlockInChain()
        {
            // Given
            using var table = CreateInstance();
            table.TryCreateBlockChain(out var firstBlock).ShouldBeTrue();

            // When
            table.TryGetNextBlockInChain(firstBlock, out var nextBlock, true).ShouldBeTrue();

            // Then
            table.GetNextBlock(ref firstBlock).ShouldBe(nextBlock);
            nextBlock.SectorId.ShouldBe(0);
            nextBlock.BlockId.ShouldBe(1);
            table.GetNextBlock(ref nextBlock).ShouldBe(Block.Last);
            
            table.NumberOfSectorsUsed.ShouldBe(1);
            table.NumberOfBlocksUsed.ShouldBe(2);
        }
        
        [Fact]
        public void ShouldCreateChain()
        {
            // Given
            using var table = CreateInstance();
            table.TryCreateBlockChain(out var firstBlock).ShouldBeTrue();

            // When
            table.TryGetNextBlockInChain(firstBlock, out var nextBlock1, true).ShouldBeTrue();
            table.TryGetNextBlockInChain( nextBlock1, out var nextBlock2, true).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock2, out var nextBlock3, true).ShouldBeTrue();
            table.TryCreateBlockChain(out _).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock3, out var nextBlock4, true).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock4, out var nextBlock5, true).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock5, out var nextBlock6, true).ShouldBeTrue();

            // Then
            nextBlock1.SectorId.ShouldBe(0);
            nextBlock1.BlockId.ShouldBe(1);
            nextBlock4.SectorId.ShouldBe(1);
            nextBlock4.BlockId.ShouldBe(1);
            nextBlock6.SectorId.ShouldBe(1);
            nextBlock6.BlockId.ShouldBe(3);
            table.GetNextBlock(ref firstBlock).ShouldBe(nextBlock1);
            table.GetNextBlock(ref nextBlock1).ShouldBe(nextBlock2);
            table.GetNextBlock(ref nextBlock2).ShouldBe(nextBlock3);
            table.GetNextBlock(ref nextBlock3).ShouldBe(nextBlock4);
            table.GetNextBlock(ref nextBlock4).ShouldBe(nextBlock5);
            table.GetNextBlock(ref nextBlock5).ShouldBe(nextBlock6);
            table.GetNextBlock(ref nextBlock6).ShouldBe(Block.Last);
            
            table.NumberOfSectorsUsed.ShouldBe(2);
            table.NumberOfBlocksUsed.ShouldBe(8);
        }
        
        [Fact]
        public void ShouldReleaseChain()
        {
            // Given
            using var table = CreateInstance();
            table.TryCreateBlockChain(out var firstBlock).ShouldBeTrue();
            table.TryGetNextBlockInChain(firstBlock, out var nextBlock1, true).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock1, out var nextBlock2, true).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock2, out var nextBlock3, true).ShouldBeTrue();
            table.TryCreateBlockChain(out _).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock3, out var nextBlock4, true).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock4, out var nextBlock5, true).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock5, out _, true).ShouldBeTrue();

            // When
            table.TryReleaseBlockChain(firstBlock);

            // Then
            table.NumberOfSectorsUsed.ShouldBe(2);
            table.NumberOfBlocksUsed.ShouldBe(1);
        }
        
        [Fact]
        public void ShouldDispose()
        {
            // Given
            using var table = CreateInstance();
            table.TryCreateBlockChain(out var firstBlock).ShouldBeTrue();
            table.TryGetNextBlockInChain(firstBlock, out var nextBlock1, true).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock1, out var nextBlock2, true).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock2, out var nextBlock3, true).ShouldBeTrue();
            table.TryCreateBlockChain(out _).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock3, out var nextBlock4, true).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock4, out var nextBlock5, true).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock5, out _, true).ShouldBeTrue();

            // When
            table.Dispose();

            // Then
            table.NumberOfSectorsUsed.ShouldBe(0);
            table.NumberOfBlocksUsed.ShouldBe(0);
        }
        
        [Fact]
        public void ShouldSaveAndLoad()
        {
            // Given
            using var table = CreateInstance();
            table.TryCreateBlockChain(out var firstBlock).ShouldBeTrue();
            table.TryGetNextBlockInChain(firstBlock, out var nextBlock1, true).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock1, out var nextBlock2, true).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock2, out var nextBlock3, true).ShouldBeTrue();
            table.TryCreateBlockChain(out _).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock3, out var nextBlock4, true).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock4, out var nextBlock5, true).ShouldBeTrue();
            table.TryGetNextBlockInChain(nextBlock5, out _, true).ShouldBeTrue();

            var dataSize = _calculator.SectorSize * 2;
            using var buffer = MemoryPool<byte>.Shared.Rent(dataSize);
            var data = buffer.Memory[..dataSize];
            data.Span.Clear();

            // When
            table.TrySave(new MemoryWriter(data)).ShouldBeTrue();
            table.Clear();
            table.TryLoad(new MemoryReader(data)).ShouldBeTrue();

            // Then
            table.NumberOfSectorsUsed.ShouldBe(2);
            table.NumberOfBlocksUsed.ShouldBe(8);
        }

        private BlockAllocationTable CreateInstance() =>
            new(this, _calculator);
    }
}