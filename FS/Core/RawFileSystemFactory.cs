namespace FS.Core
{
    internal sealed class RawFileSystemFactory: IFactory<RawFileSystemSettings, IRawFileSystem>
    {
        private readonly IContext<RawFileSystemSettings> _context;

        public RawFileSystemFactory(IContext<RawFileSystemSettings> context) => _context = context;

        public IRawFileSystem Create(RawFileSystemSettings data)
        {
            using (_context.Apply(data))
            {
                return Composer.Resolve<IRawFileSystem>();
            }
        }
    }
}