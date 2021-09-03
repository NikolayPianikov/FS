namespace FS
{
    public interface IReaderWriterFactory<in T>
    {
        public IReader CreateReader(T file);

        public IWriter CreateWriter(T file);
    }
}