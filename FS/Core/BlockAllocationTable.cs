// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable BuiltInTypeReferenceStyle
namespace FS.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    internal sealed class BlockAllocationTable:
        IBlockAllocationTable,
        IBlockAllocationTableStatistics
    {
        // ReSharper disable once ArrangeDefaultValueWhenTypeEvident
        // ReSharper disable once MemberCanBePrivate.Global
        private readonly IMemoryManager<Block> _memoryManager;
        private readonly List<Sector> _sectors = new();
        private readonly IBlockCalculator _calculator;

        public BlockAllocationTable(IMemoryManager<Block> memoryManager, IBlockCalculator calculator)
        {
            _memoryManager = memoryManager;
            _calculator = calculator;
        }

        public int NumberOfSectorsUsed => _sectors.Count;

        public int NumberOfBlocksUsed
        {
            get
            {
                var count = 0;
                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                foreach (var sector in _sectors)
                {
                    foreach (var block in sector.Data.Span)
                    {
                        if (!block.Equals(Block.Empty))
                        {
                            count++;
                        }
                    }
                }

                return count;
            }
        }

        public bool TryCreateBlockChain(out Block firstBlock) => TryReserveBlock(out firstBlock);

        public bool TryGetNextBlockInChain(Block currentBlock, out Block nextBlock, bool expansion)
        {
            GuardBlock(ref currentBlock);
            if (TryGetNextBlockInChain(ref currentBlock, out nextBlock))
            {
                return true;
            }

            if (!expansion)
            {
                return false;
            }

            if (!TryReserveBlock(out nextBlock))
            {
                return false;
            }

            SetNextBlock(ref currentBlock, nextBlock);
            SetNextBlock(ref nextBlock, Block.Last);
            return true;
        }

        public bool TryReleaseBlockChain(Block firstBlock)
        {
            var currentBlock = firstBlock;
            while (TryGetNextBlockInChain(currentBlock, out var nextBlock, false))
            {
                GuardBlockNotEmpty(ref nextBlock);
                SetNextBlock(ref currentBlock, Block.Empty);
                currentBlock = nextBlock;
            }
            
            GuardBlockNotEmpty(ref currentBlock);
            if (GetNextBlock(ref currentBlock).Equals(Block.Empty))
            {
                return false;
            }
            
            SetNextBlock(ref currentBlock, Block.Empty);
            return true;
        }

        public bool TrySave(IWriter writer)
        {
            var hasBusy = false;
            for (var sectorId = _sectors.Count - 1; sectorId >= 0; sectorId--)
            {
                var sector = _sectors[sectorId];
                if (!hasBusy)
                {
                    foreach (var block in sector.Data.Span)
                    {
                        // ReSharper disable once InvertIf
                        if (!block.Equals(Block.Empty))
                        {
                            hasBusy = true;
                            break;
                        }
                    }

                    if (!hasBusy)
                    {
                        _sectors.RemoveAt(sectorId);
                    }
                }
                
                if (!sector.IsDirty)
                {
                    continue;
                }
                
                var sectorPosition = _calculator.SectorToPosition(sectorId);
                var data = MemoryMarshal.AsBytes(sector.Data.Span);
                if (writer.Write(data, sectorPosition) != data.Length)
                {
                    return false;
                }

                sector.IsDirty = false;
            }

            return true;
        }
        
        public bool TryLoad(IReader reader)
        {
            Clear();
            bool hasNewSector;
            var sectorId = 0;
            do
            {
                var sectorPosition = _calculator.SectorToPosition(sectorId);
                if (!TryCreateSector(out var newSector))
                {
                    Clear();
                    return false;
                }

                var buffer = MemoryMarshal.AsBytes(newSector.Data.Span);
                hasNewSector = reader.Read(sectorPosition, buffer) == buffer.Length;
                if (hasNewSector)
                {
                    _sectors.Add(newSector);
                }

                sectorId++;
            } while (hasNewSector);

            return true;
        }
        
        public void Dispose() => Clear();

        internal void Clear()
        {
            foreach (var sector in _sectors)
            {
                sector.Recourse.Dispose();
            }

            _sectors.Clear();
        }

        [Conditional("DEBUG")]
        private void GuardBlock(ref Block block)
        {
            if (block.SectorId >= _sectors.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(block.SectorId));
            }
            
            if (block.BlockId >= _calculator.BlockCount)
            {
                throw new ArgumentOutOfRangeException(nameof(block.BlockId));
            }
        }
        
        [Conditional("DEBUG")]
        private void GuardBlockNotEmpty(ref Block block)
        {
            if (GetNextBlock(ref block).Equals(Block.Empty))
            {
                throw new ArgumentOutOfRangeException(nameof(block), $"{block} points to an empty block.");
            }
        }

        internal Block GetNextBlock(ref Block block)
        {
            GuardBlock(ref block);
            return _sectors[block.SectorId].Data.Span[block.BlockId];
        }

        private void SetNextBlock(ref Block block, Block nextBlock)
        {
            GuardBlock(ref block);
            var sector = _sectors[block.SectorId];
            sector.Data.Span[block.BlockId] = nextBlock;
            sector.IsDirty = true;
        }

        private bool TryGetNextBlockInChain(ref Block currentBlock, out Block nextBlock)
        {
            GuardBlock(ref currentBlock);
            nextBlock = GetNextBlock(ref currentBlock);
            // ReSharper disable once InvertIf
            if (nextBlock.Equals(Block.Empty) || nextBlock.Equals(Block.Last))
            {
                nextBlock = default;
                return false;
            }

            return true;
        }
        
        private bool TryReserveBlock(out Block block)
        {
            for (var sectorId = 0; sectorId < _sectors.Count; sectorId++)
            {
                var sector = _sectors[sectorId];
                var blockId = sector.Data.Span.IndexOf(Block.Empty);
                // ReSharper disable once InvertIf
                if (blockId >= 0)
                {
                    block = new Block(sectorId, blockId);
                    SetNextBlock(ref block, Block.Last);
                    return true;
                }
            }

            if (TryCreateSector(out var newSector))
            {
                _sectors.Add(newSector);
                block = new Block(_sectors.Count - 1, 0);
                SetNextBlock(ref block, Block.Last);
                return true;
            }

            block = default;
            return false;
        }

        private bool TryCreateSector(out Sector sector)
        {
            var buffer = _memoryManager.Rent(_calculator.BlockCount);
            var data = buffer.Memory[.._calculator.BlockCount];
            data.Span.Fill(Block.Empty);
            sector = new Sector(data, buffer);
            return true;
        }
        
        private class Sector
        {
            public readonly Memory<Block> Data;
            public readonly IDisposable Recourse;
            public bool IsDirty;

            public Sector(Memory<Block> data, IDisposable recourse)
            {
                Data = data;
                Recourse = recourse;
                IsDirty = true;
            }
        }
    }
}