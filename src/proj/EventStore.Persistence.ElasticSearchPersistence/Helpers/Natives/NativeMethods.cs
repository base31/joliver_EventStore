using System;
using System.Runtime.InteropServices;

namespace EventStore.Persistence.ElasticSearchPersistence.Helpers.Natives
{
    public class NativeMethods
    {
        [DllImport("rpcrt4.dll", SetLastError = true)]
        public static extern int UuidCreateSequential(out Guid guid);
    }
}