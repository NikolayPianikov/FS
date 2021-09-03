namespace FS.Core
{
    using System.IO;

    internal sealed class FileReaderWriterFactory: IReaderWriterFactory<string>
    {
        public IReader CreateReader(string file) => new StreamReader(CreateStream(file));

        public IWriter CreateWriter(string file) => new StreamWriter(CreateStream(file));

        private static FileStream CreateStream(string file) => File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
    }
}