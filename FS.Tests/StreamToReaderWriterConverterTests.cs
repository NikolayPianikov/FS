namespace FS.Tests
{
    using System.IO;
    using Core;
    using Shouldly;
    using Xunit;

    public class StreamToReaderWriterConverterTests
    {
        [Fact]
        public void ShouldConvertStreamToReader()
        {
            // Given
            var converter = CreateInstance();
            var (source, destination, resource) = TestTool.CreateBuffers(256);
            using (resource)
            {
                // When
                var reader = ((IConverter<Stream, IReader>)converter).Convert(new MemoryStream(source.ToArray()));
                reader.Read(0, destination.Span);

                // Then
                destination.ShouldBe(source);
            }
        }
        
        [Fact]
        public void ShouldConvertStreamToWriter()
        {
            // Given
            var converter = CreateInstance();
            var (source, destination, resource) = TestTool.CreateBuffers(256);
            using (resource)
            {
                // When
                var stream = ((IConverter<Stream, IWriter>)converter).Convert(new ReaderWriterStream(null, new MemoryWriter(destination)));
                stream.Write(source.Span, 0);

                // Then
                destination.ShouldBe(source);
            }
        }

        private static StreamToReaderWriterConverter CreateInstance() =>
            new();
    }
}