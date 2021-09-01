namespace FS.Core
{
    using System;

    internal interface IBlockAllocationTable: IDisposable
    {
        bool TryCreateBlockChain(out Block firstBlock);
        
        bool TryGetNextBlockInChain(Block currentBlock, out Block nextBlock, bool expansion);
        
        bool TryReleaseBlockChain(Block firstBlock);
        
        bool TrySave(IWriter writer);

        bool TryLoad(IReader reader);
    }
}