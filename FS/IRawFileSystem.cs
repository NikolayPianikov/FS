namespace FS
{
    public interface IRawFileSystem
    {
        bool TryCreateFile(out RawFile file);
        
        void DeleteFile(RawFile file);
        
        IReader CreateReader(RawFile file);
        
        IWriter CreateWriter(RawFile file);
    }
}