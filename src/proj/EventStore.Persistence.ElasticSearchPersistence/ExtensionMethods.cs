using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EventStore.Persistence.ElasticSearchPersistence.Helpers;
using EventStore.Serialization;

namespace EventStore.Persistence.ElasticSearchPersistence
{
    public static class ExtensionMethods
    {
        public static ElasticSearchCommit ToElasticSearchCommit(this Commit commit, IDocumentSerializer serializer)
        {
            return new ElasticSearchCommit
            {
                Id = ToElasticSearchId(commit),
                CommitId = ToDashlessGuid(commit.CommitId),
                CommitSequence = commit.CommitSequence,
                CommitStamp = commit.CommitStamp,
                StreamId = ToDashlessGuid(commit.StreamId),
                StartingStreamRevision = commit.StreamRevision - (commit.Events.Count - 1),
                StreamRevision = commit.StreamRevision,
                Headers = Convert.ToBase64String(SerializerHelper.ObjectToByteArray(commit.Headers)),//SerializerHelper.ProtoObjectToByteArray(header)),
                Payload = Convert.ToBase64String(SerializerHelper.ObjectToByteArray(commit.Events))//SerializerHelper.ProtoObjectToByteArray(data))
            };
        }

        private static string ToElasticSearchId(Commit commit)
        {
            return GuidHelper.ToNewDeterministicGuid(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", 
                commit.StreamId.ToString("N"), commit.CommitSequence)).ToString("N");
        }

        public static Commit ToCommit(this ElasticSearchCommit commit)
        {
            var headers =(Dictionary<string, object>)
                SerializerHelper.ByteArrayToObject(
                    Convert.FromBase64String(commit.Headers));

            var events = (List<EventMessage>)
                SerializerHelper.ByteArrayToObject(
                Convert.FromBase64String(commit.Payload));

            return new Commit(
                ToGuid(commit.StreamId),
                commit.StreamRevision,
                ToGuid(commit.CommitId),
                commit.CommitSequence,
                commit.CommitStamp,
                headers,
                events);
        }

        public static IEnumerable<Commit> ToCommits(this IEnumerable<ElasticSearchCommit> elasticSearchCommits)
        {
            return elasticSearchCommits.Select(elasticCommit => elasticCommit.ToCommit()).ToList();
        }

        private static string ToDashlessGuid(Guid guid)
        {
            return guid.ToString("N");
        }

        private static Guid ToGuid(string stringGuid)
        {
            return GuidHelper.NewGuidFromStringWithoutDashes(stringGuid);
        }
    }
}