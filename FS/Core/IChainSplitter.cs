namespace FS.Core
{
    using System.Collections.Generic;

    internal interface IChainSplitter
    {
        IEnumerable<DataRange> Split(Block firstBlock, long position, long size, bool expansion);
    }
}