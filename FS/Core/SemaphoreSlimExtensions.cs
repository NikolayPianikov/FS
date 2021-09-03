namespace FS.Core
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class SemaphoreSlimExtensions
    {
        public static SemaphoreSlim CreateSyncSemaphore() => new(1, 1);

        public static IDisposable CreateSyncToken(this SemaphoreSlim semaphore)
        {
            semaphore.Wait();
            return Disposable.Create(() => semaphore.Release());
        }
        
        public static async Task<IDisposable> CreateSyncTokenAsync(this SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            return Disposable.Create(() => semaphore.Release());
        }
    }
}