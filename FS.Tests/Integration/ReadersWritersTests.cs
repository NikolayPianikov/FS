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
            var data = buffer.Memory[..DataSize];
            data.Span.Clear();
            var memoryWriter = new MemoryWriter(data);
            var memoryReader = new MemoryReader(data);
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
                var source = sourceBuffer.Memory[..Size];
                source.Span.Clear();
                source.Fill();
                var destination = destinationBuffer.Memory[..Size];
                destination.Span.Clear();

                // When
                writer.Write(source.Span, Position);
                var result = reader.Read(Position, destination.Span);

                // Then
                result.ShouldBe(Size);
                source.ShouldBe(destination);
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
                var destination = destinationBuffer.Memory[..Size];
                destination.Span.Clear();

                // When
                await writer.WriteAsync(data, Position);
                var result = await reader.ReadAsync(Position, destination);

                // Then
                result.ShouldBe(Size);
                data.ShouldBe(destination);
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