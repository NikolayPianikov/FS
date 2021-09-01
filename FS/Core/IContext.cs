namespace FS.Core
{
    using System;

    internal interface IContext<T>
    {
        T Data { get; }

        IDisposable Apply(T data);
    }
}