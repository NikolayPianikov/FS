namespace FS.Tests.Integration
{
    using System.Buffers;
    using System.IO;
    using System.Threading.Tasks;
    using Core;
    using Shouldly;
    using Xunit;

    public class RawFileSystemTests
    {
        private const int Size = 256000;
        private const int Position = 123;

        [Fact]
        public void ShouldReadAndWriteFile()
        {
            // Given
            var tempFile = Path.GetTempFileName();
            try
            {
                using var stream = File.Open(tempFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                var fileSystem = CreateInstance(new RawFileSystemSettings(new FileReader(stream), new FileWriter(stream), 256, 16));
                using var sourceBuffer = MemoryPool<byte>.Shared.Rent(Size);
                using var destinationBuffer = MemoryPool<byte>.Shared.Rent(Size);
                var data = sourceBuffer.Memory[..Size];
                data.Fill();

                // When
                fileSystem.TryCreateFile(out var file).ShouldBeTrue();
                var writer = fileSystem.CreateWriter(file);
                writer.Write(data.Span, Position);
                writer.TryFlush().ShouldBeTrue();

                var reader = fileSystem.CreateReader(file);
                var result = reader.Read(Position, destinationBuffer.Memory.Span);

                // Then
                result.ShouldBe(Size);
                data.ShouldBe(destinationBuffer.Memory[..Size]);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
        
        [Fact]
        public async Task ShouldReadAndWriteFileAsync()
        {
            // Given
            var tempFile = Path.GetTempFileName();
            try
            {
                await using var stream = File.Open(tempFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                var fileSystem = CreateInstance(new RawFileSystemSettings(new FileReader(stream), new FileWriter(stream), 256, 16));
                using var sourceBuffer = MemoryPool<byte>.Shared.Rent(Size);
                using var destinationBuffer = MemoryPool<byte>.Shared.Rent(Size);
                var data = sourceBuffer.Memory[..Size];
                data.Fill();

                // When
                fileSystem.TryCreateFile(out var file).ShouldBeTrue();
                var writer = fileSystem.CreateWriter(file);
                await writer.WriteAsync(data, Position);
                (await writer.TryFlushAsync()).ShouldBeTrue();

                var reader = fileSystem.CreateReader(file);
                var result = await reader.ReadAsync(Position, destinationBuffer.Memory);

                // Then
                result.ShouldBe(Size);
                data.ShouldBe(destinationBuffer.Memory[..Size]);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        private static IRawFileSystem CreateInstance(RawFileSystemSettings settings) =>
            Composer.Resolve<IFactory<RawFileSystemSettings, IRawFileSystem>>().Create(settings);
    }
}