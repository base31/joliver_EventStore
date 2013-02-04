using System.Collections.Generic;
using ProtoBuf;

namespace EventStore.Persistence.ElasticSearchPersistence
{
    [ProtoContract]
    public class Header
    {
        [ProtoMember(1)]
        public byte[] Headers { get; set; }
    }
}