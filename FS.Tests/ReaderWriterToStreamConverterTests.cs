namespace FS.Tests
{
    using Core;
    using Shouldly;
    using Xunit;

    public class ReaderWriterToStreamConverterTests
    {
        [Fact]
        public void ShouldConvertReaderToStream()
        {
            // Given
            var converter = CreateInstance();
            var (source, destination, resource) = TestTool.CreateBuffers(256);
            using (resource)
            {
                // When
                var stream = converter.Convert(new MemoryReader(source));
                stream.Read(destination.Span);

                // Then
                destination.ShouldBe(source);
            }
        }
        
        [Fact]
        public void ShouldConvertWriterToStream()
        {
            // Given
            var converter = CreateInstance();
            var (source, destination, resource) = TestTool.CreateBuffers(256);
            using (resource)
            {
                // When
                var stream = converter.Convert(new MemoryWriter(destination));
                stream.Write(source.Span);

                // Then
                destination.ShouldBe(source);
            }
        }

        private static ReaderWriterToStreamConverter CreateInstance() =>
            new();
    }
}