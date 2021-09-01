// ReSharper disable UnusedMemberInSuper.Global
namespace FS
{
    public interface IRawFileSystem
    {
        bool TryCreateFile(out RawFile file);
        
        bool TryDeleteFile(RawFile file);
        
        IReader CreateReader(RawFile file);
        
        IWriter CreateWriter(RawFile file);
    }
}