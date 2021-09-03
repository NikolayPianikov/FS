namespace FS.Tests.Integration
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Core;
    using Shouldly;
    using Xunit;
    using StreamReader = Core.StreamReader;
    using StreamWriter = Core.StreamWriter;

    public class ReadersWritersTests
    {
        private const int Size = 25600;
        private const int Position = 123;
        private const int DataSize = Size + Position;
        
        public static IEnumerable<object?[]> GetData()
        {
            // File system
            var tempFile = Path.GetTempFileName();
            var tempFileToken = Disposable.Create(() => File.Delete(tempFile));
            var stream = File.Open(tempFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            var fileWriter = new StreamWriter(stream);
            var fileReader = new StreamReader(stream);
            yield return new object[] { fileReader, fileWriter, Disposable.Create(stream, tempFileToken) };

            // Memory
            var buffer = MemoryPool<byte>.Shared.Rent(DataSize);
            var memoryWriter = new MemoryWriter(buffer.Memory[..DataSize]);
            var memoryReader = new MemoryReader(buffer.Memory[..DataSize]);
            yield return new object[] { memoryReader, memoryWriter, buffer };
            
            // Aaa
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void ShouldReadWrite(IReader reader, IWriter writer, IDisposable resource)
        {
            using (resource)
            {
                // Given
                using var sourceBuffer = MemoryPool<byte>.Shared.Rent(Size);
                using var destinationBuffer = MemoryPool<byte>.Shared.Rent(Size);
                var data = sourceBuffer.Memory[..Size];
                data.Fill();

                // When
                writer.Write(data.Span, Position);
                var result = reader.Read(Position, destinationBuffer.Memory.Span);

                // Then
                result.ShouldBe(Size);
                data.ShouldBe(destinationBuffer.Memory[..Size]);
            }
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public async Task ShouldReadWriteAsync(IReader reader, IWriter writer, IDisposable resource)
        {
            using (resource)
            {
                // Given
                using var sourceBuffer = MemoryPool<byte>.Shared.Rent(Size);
                using var destinationBuffer = MemoryPool<byte>.Shared.Rent(Size);
                var data = sourceBuffer.Memory[..Size];
                data.Fill();

                // When
                await writer.WriteAsync(data, Position);
                var result = await reader.ReadAsync(Position, destinationBuffer.Memory);

                // Then
                result.ShouldBe(Size);
                data.ShouldBe(destinationBuffer.Memory[..Size]);
            }
        }

        [Fact]
        // ReSharper disable once InconsistentNaming
        public void ShouldThrowIOExceptionWhenOutOfMemory()
        {
            // Given
            var buffer = MemoryPool<byte>.Shared.Rent(Size);
            var memoryWriter = new MemoryWriter(buffer.Memory);
            using var sourceBuffer = MemoryPool<byte>.Shared.Rent(Size);
            using var destinationBuffer = MemoryPool<byte>.Shared.Rent(Size);
            sourceBuffer.Memory.Fill();

            // When

            // Then
            Should.Throw<IOException>(() => memoryWriter.Write(sourceBuffer.Memory.Span, Position));
        }
    }
}