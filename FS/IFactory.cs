namespace FS
{
    public interface IFactory<in TData, out T>
    {
        T Create(TData data);
    }
}