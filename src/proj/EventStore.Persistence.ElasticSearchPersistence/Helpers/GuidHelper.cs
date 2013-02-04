using System;
using System.Security.Cryptography;
using System.Text;
using EventStore.Persistence.ElasticSearchPersistence.Helpers.Natives;

namespace EventStore.Persistence.ElasticSearchPersistence.Helpers
{
    public static class GuidHelper
    {
        public static Guid NewSequentialGuid()
        {
            const int RPC_S_OK = 0;

            Guid guid;
            int result = NativeMethods.UuidCreateSequential(out guid);
            if (result == RPC_S_OK)
                return guid;
            return Guid.NewGuid();
        }

        public static Guid NewDeterministicGuid(string value)
        {
            var provider = new MD5CryptoServiceProvider();
            var inputBytes = Encoding.Default.GetBytes(value);
            var hashBytes = provider.ComputeHash(inputBytes);
            return new Guid(hashBytes);
        }

        public static Guid ToNewDeterministicGuid(string src)
        {
            byte[] stringbytes = Encoding.UTF8.GetBytes(src);
            byte[] hashedBytes = new SHA1CryptoServiceProvider()
                .ComputeHash(stringbytes);
            Array.Resize(ref hashedBytes, 16);
            return new Guid(hashedBytes);
        }

        public static string NewStringFromSequentialGuid()
        {
            return NewSequentialGuid().ToString("N");
        }

        public static Guid NewGuidFromStringWithoutDashes(string stringGuid)
        {
            return Guid.Parse(stringGuid.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-"));
        }
    }
}