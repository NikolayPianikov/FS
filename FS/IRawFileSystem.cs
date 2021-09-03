// ReSharper disable UnusedMemberInSuper.Global
namespace FS
{
    public interface IRawFileSystem: IReaderWriterFactory<RawFile>
    {
        bool TryCreateFile(out RawFile file);
        
        bool TryDeleteFile(RawFile file);
        
        bool TryFlush();
    }
}