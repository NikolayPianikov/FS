namespace FS.Core
{
    using System;

    internal sealed class Context<T> : IContext<T>
        where T: struct
    {
        private T _data;

        public T Data
        {
            get
            {
                if (Equals(_data, default(T)))
                {
                    throw new InvalidOperationException($"{typeof(T).Name} is not defined.");
                }

                return _data;
            }
        }

        public IDisposable Apply(T data)
        {
            var prevData = _data;
            _data = data;
            return Disposable.Create(() => _data = prevData);
        }
    }
}