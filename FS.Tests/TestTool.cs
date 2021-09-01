namespace FS.Tests
{
    using System;

    internal static class TestTool
    {
        public static void Fill(this Memory<byte> memory)
        {
            for (var i = 0; i < memory.Length; i++)
            {
                memory.Span[i] = (byte)(i + 1);
            }
        }
    }
}