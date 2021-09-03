namespace FS.Core
{
    using System.IO;

    internal sealed class ReaderWriterToStreamConverter : IConverter<IReader, Stream>, IConverter<IWriter, Stream>
    {
        public Stream Convert(IReader reader) => new ReaderWriterStream(reader, default);

        public Stream Convert(IWriter writer) => new ReaderWriterStream(default, writer);
    }
}