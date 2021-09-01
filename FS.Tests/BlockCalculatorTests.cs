// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable UselessBinaryOperation
namespace FS.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Core;
    using Shouldly;
    using Xunit;
    using Position = System.Int64;

    public class BlockCalculatorTests
    {
        private const int BlockSize = 2;
        private const int BlockCount = 4;
        private const int DataSize = BlockSize * BlockCount;
        private static readonly int HeaderSize = Marshal.SizeOf<Block>() * BlockCount;
        private static readonly int SectorSize = HeaderSize + DataSize;
        
        [Fact]
        public void ShouldProvideGeneralSettings()
        {
            // Given

            // When
            var calculator = CreateInstance();

            // Then
            calculator.BlockSize.ShouldBe(BlockSize);
            calculator.BlockCount.ShouldBe(BlockCount);
            calculator.SectorSize.ShouldBe(SectorSize);
        }
        
        public static IEnumerable<object?[]> GetConvertSectorToPositionCases()
        {
            yield return new object[] { 1, SectorSize };
            yield return new object[] { 3, SectorSize * 3 };
            yield return new object[] { 0, 0 };
        }

        [Theory]
        [MemberData(nameof(GetConvertSectorToPositionCases))]
        internal void ShouldConvertSectorToPosition(int sectorId, Position expectedPosition)
        {
            // Given
            var calculator = CreateInstance();

            // When
            var actualPosition = calculator.SectorToPosition(sectorId);

            // Then
            actualPosition.ShouldBe(expectedPosition);
        }
        
        public static IEnumerable<object?[]> GetConvertBlockToPositionCases()
        {
            yield return new object[] { new Block(1, 1), SectorSize * 1 + HeaderSize + BlockSize * 1, false };
            yield return new object[] { new Block(0, 1), SectorSize * 0 + HeaderSize + BlockSize * 1, false };
            yield return new object[] { new Block(4, 3), SectorSize * 4 + HeaderSize + BlockSize * 3, false };
            yield return new object[] { new Block(0, 0), SectorSize * 0 + HeaderSize + BlockSize * 0, false };
            yield return new object[] { new Block(2, 4), SectorSize * 2 + HeaderSize + BlockSize * 4, true };
            yield return new object[] { new Block(1, 5), SectorSize * 1 + HeaderSize + BlockSize * 5, true };
            yield return new object[] { new Block(0, 4), SectorSize * 0 + HeaderSize + BlockSize * 4, true };
        }

        [Theory]
        [MemberData(nameof(GetConvertBlockToPositionCases))]
        internal void ShouldConvertBlockToPosition(Block block, Position expectedPosition, bool expectedException)
        {
            // Given
            var calculator = CreateInstance();

            // When
            try
            {
                var actualPosition = calculator.BlockToPosition(block);

                // Then
                actualPosition.ShouldBe(expectedPosition);
            }
            catch(ArgumentOutOfRangeException)
            {
                expectedException.ShouldBeTrue();
            }
        }

        private static BlockCalculator CreateInstance() =>
            new(BlockSize, BlockCount);
    }
}