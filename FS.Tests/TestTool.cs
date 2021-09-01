namespace FS.Tests
{
    using System;
    using System.Buffers;
    using System.Threading.Tasks;

    internal static class TestTool
    {
        public static void Fill(this Memory<byte> memory)
        {
            for (var i = 0; i < memory.Length; i++)
            {
                memory.Span[i] = (byte)(i + 1);
            }
        }

        public static bool ReadAndWriteFile(IReader reader, IWriter writer, int position, int size)
        {
            using var sourceBuffer = MemoryPool<byte>.Shared.Rent(size);
            using var destinationBuffer = MemoryPool<byte>.Shared.Rent(size);
            var data = sourceBuffer.Memory[..size];
            data.Fill();
            var result1 = writer.Write(data.Span, position);
            var result2 = reader.Read(position, destinationBuffer.Memory.Span);
            return 
                result1 == size
                && result2 == size 
                && data.Span.SequenceEqual(destinationBuffer.Memory[..size].Span);
        }
        
        public static async Task<bool> ReadAndWriteFileAsync(IReader reader, IWriter writer, int position, int size)
        {
            using var sourceBuffer = MemoryPool<byte>.Shared.Rent(size);
            using var destinationBuffer = MemoryPool<byte>.Shared.Rent(size);
            var data = sourceBuffer.Memory[..size];
            data.Fill();
            var result1 = await writer.WriteAsync(data, position);
            var result2 = await reader.ReadAsync(position, destinationBuffer.Memory);
            return 
                result1 == size
                && result2 == size 
                && data.Span.SequenceEqual(destinationBuffer.Memory[..size].Span);
        }
    }
}