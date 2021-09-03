namespace FS.Tests
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Shouldly;

    internal static class TestTool
    {
        public static void Fill(this Memory<byte> memory)
        {
            for (var i = 0; i < memory.Length; i++)
            {
                memory.Span[i] = (byte)(i + 1);
            }
        }
        
        public static void Parallelize(Action action, int count = 10, int parallelism = 100)
        {
            var exceptions = new List<Exception>();
            void RunAction(object state)
            {
                try
                {
                    for (var i = 0; i < count; i++)
                    {
                        action();
                    }
                }
                catch (Exception ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            }

            var threads = new List<Thread>();
            for (var i = 0; i < parallelism; i++)
            {
                threads.Add(new Thread(RunAction!) { IsBackground = true });
            }

            threads.ForEach(i => i.Start());
            threads.ForEach(i => i.Join());

            if (exceptions.Count > 0)
            {
                throw exceptions[0];
            }
        }

        private static string ConvertToString<T>(this Span<T> span)
        {
            var sb = new StringBuilder();
            foreach (var item in span)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }
                
                sb.Append(item);
            }

            return sb.ToString();
        }
        
        public static void ReadAndWriteFile(IReader reader, IWriter writer, int position, int size)
        {
            using var sourceBuffer = MemoryPool<byte>.Shared.Rent(size);
            var data = sourceBuffer.Memory[..size];
            data.Fill();
            writer.Write(data.Span, position).ShouldBe(size);
            writer.Flush();

            using var destinationBuffer = MemoryPool<byte>.Shared.Rent(size);
            var destination = destinationBuffer.Memory[..size];
            reader.Read(position, destination.Span).ShouldBe(size, "Expected: " + data.Span.ConvertToString() + ", actual: " + destination.Span.ConvertToString());
            data.ShouldBe(destination);
        }
        
        public static async Task ReadAndWriteFileAsync(IReader reader, IWriter writer, int position, int size)
        {
            using var sourceBuffer = MemoryPool<byte>.Shared.Rent(size);
            var data = sourceBuffer.Memory[..size];
            data.Fill();
            (await writer.WriteAsync(data, position)).ShouldBe(size);
            await writer.FlushAsync();

            using var destinationBuffer = MemoryPool<byte>.Shared.Rent(size);
            var destination = destinationBuffer.Memory[..size];
            (await reader.ReadAsync(position, destination)).ShouldBe(size, "Expected: " + data.Span.ConvertToString() + ", actual: " + destination.Span.ConvertToString());
            data.ShouldBe(destination);
        }
    }
}