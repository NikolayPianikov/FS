namespace FS.Core
{
    using System.IO;

    internal sealed class StreamToReaderWriterConverter: IConverter<Stream, IReader>, IConverter<Stream, IWriter>
    {
        IReader IConverter<Stream, IReader>.Convert(Stream stream) => new StreamReader(stream);

        IWriter IConverter<Stream, IWriter>.Convert(Stream stream) => new StreamWriter(stream);
    }
}