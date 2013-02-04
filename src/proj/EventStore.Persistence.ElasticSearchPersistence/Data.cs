using System.Collections.Generic;
using ProtoBuf;

namespace EventStore.Persistence.ElasticSearchPersistence
{
    [ProtoContract]
    public class Data
    {
        [ProtoMember(1)]
        public byte[] Events { get; set; }
    }
}