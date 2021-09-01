namespace FS.Core
{
    using System;

    internal interface IBlockAllocationTable: IDisposable
    {
        bool CreateBlockChain(out Block firstBlock);
        
        bool TryGetNextBlockInChain(Block currentBlock, out Block nextBlock, bool expansion);
        
        void ReleaseBlockChain(Block firstBlock);
        
        bool TrySave(IWriter writer);

        bool TryLoad(IReader reader);
    }
}