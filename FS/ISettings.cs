namespace FS
{
    // ReSharper disable once IdentifierTypo
    public interface ISettings<out T, in TSettings>
    {
        T Apply(TSettings settings);
    }
}