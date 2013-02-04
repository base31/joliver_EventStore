using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

namespace EventStore.Persistence.ElasticSearchPersistence.Helpers
{
    public static class SerializerHelper
    {
        public static byte[] ProtoObjectToByteArray<T>(T obj)
        {
            using (var mem = new MemoryStream())
            {
                Serializer.Serialize(mem, obj);
                return mem.ToArray();
            }
        }

        public static T ProtoByteArrayToObject<T>(byte[] arrBytes)
        {
            //var memStream = new MemoryStream();
            //memStream.Write(arrBytes, 0, arrBytes.Length);
            //memStream.Seek(0, SeekOrigin.Begin);

            return Serializer.Deserialize<T>(new MemoryStream(arrBytes));
        }

        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = binForm.Deserialize(memStream);
            return obj;
        }
    }
}