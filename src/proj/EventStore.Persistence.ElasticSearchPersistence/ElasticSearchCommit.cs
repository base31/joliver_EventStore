using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Nest;
using Newtonsoft.Json;

namespace EventStore.Persistence.ElasticSearchPersistence
{
    [ElasticType(
    Name = "eventStoreCommit",
    DateDetection = true,
    NumericDetection = true,
    SearchAnalyzer = "standard",
    IndexAnalyzer = "standard",
    DynamicDateFormats = new[] { "dateOptionalTime", "yyyy/MM/dd HH:mm:ss Z||yyyy/MM/dd Z" })]
    [DataContract, Serializable]
    public class ElasticSearchCommit
    {
        [ElasticProperty(Index = FieldIndexOption.not_analyzed, Store = true)]
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [ElasticProperty(Index = FieldIndexOption.not_analyzed, Store = true)]
        [DataMember(Order = 2)]
        public string StreamId { get; set; }

        [ElasticProperty(Index = FieldIndexOption.not_analyzed, Store = true)]
        [DataMember(Order = 3)]
        public int CommitSequence { get; set; }

        [ElasticProperty(Index = FieldIndexOption.not_analyzed, Store = true)]
        [DataMember(Order = 4)]
        public int StartingStreamRevision { get; set; }

        [ElasticProperty(Index = FieldIndexOption.not_analyzed, Store = true)]
        [DataMember(Order = 5)]
        public int StreamRevision { get; set; }

        [ElasticProperty(Index = FieldIndexOption.not_analyzed, Store = true)]
        [DataMember(Order = 6)]
        public string CommitId { get; set; }

        [ElasticProperty(Index = FieldIndexOption.not_analyzed, Store = true)]
        [DataMember(Order = 7)]
        public DateTime CommitStamp { get; set; }

        [ElasticProperty(Index = FieldIndexOption.no, Store = false, IncludeInAll = false)]
        [DataMember(Order = 8)]
        public string Headers { get; set; }

        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        [ElasticProperty(Index = FieldIndexOption.no, Store = false, IncludeInAll = false)]
        [DataMember(Order = 9)]
        public string Payload { get; set; }

        [ElasticProperty(Index = FieldIndexOption.not_analyzed, Store = true)]
        [DataMember(Order = 10)]
        public bool Dispatched { get; set; }
    }
}