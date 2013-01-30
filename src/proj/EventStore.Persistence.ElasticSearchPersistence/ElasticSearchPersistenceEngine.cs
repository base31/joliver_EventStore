using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Persistence.ElasticSearchPersistence
{
    public class ElasticSearchPersistenceEngine : IPersistStreams
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Commit> GetFrom(Guid streamId, int minRevision, int maxRevision)
        {
            throw new NotImplementedException();
        }

        public void Commit(Commit attempt)
        {
            throw new NotImplementedException();
        }

        public Snapshot GetSnapshot(Guid streamId, int maxRevision)
        {
            throw new NotImplementedException();
        }

        public bool AddSnapshot(Snapshot snapshot)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StreamHead> GetStreamsToSnapshot(int maxThreshold)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Commit> GetFrom(DateTime start)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Commit> GetFromTo(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Commit> GetUndispatchedCommits()
        {
            throw new NotImplementedException();
        }

        public void MarkCommitAsDispatched(Commit commit)
        {
            throw new NotImplementedException();
        }

        public void Purge()
        {
            throw new NotImplementedException();
        }
    }
}
