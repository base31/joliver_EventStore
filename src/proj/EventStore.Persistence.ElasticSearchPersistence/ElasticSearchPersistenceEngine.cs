using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Logging;
using EventStore.Serialization;
using Nest;

namespace EventStore.Persistence.ElasticSearchPersistence
{
    public class ElasticSearchPersistenceEngine : IPersistStreams
    {
        
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(ElasticSearchPersistenceEngine));
        private readonly IElasticSearchPersistenceConfiguration _elasticSearchPersistenceConfiguration;
        readonly ConnectionSettings _connectionSettings;
        readonly IndexSettings _indexSettings;
        private ElasticClient _client;
        private bool _disposed;
        private int _initialized;
        private readonly IDocumentSerializer _serializer;

        public ElasticSearchPersistenceEngine(IElasticSearchPersistenceConfiguration elasticSearchPersistenceConfiguration, IDocumentSerializer serializer)
        {
            _elasticSearchPersistenceConfiguration = elasticSearchPersistenceConfiguration;
            _serializer = serializer;
            _connectionSettings = _elasticSearchPersistenceConfiguration.ConnectionSettings;
            _indexSettings = _elasticSearchPersistenceConfiguration.IndexSettings;
            _client = _elasticSearchPersistenceConfiguration.ElasticClient;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            Logger.Debug(Messages.ShuttingDownPersistence);
            _client = null;
            _disposed = true;
        }

        public IEnumerable<Commit> GetFrom(Guid streamId, int minRevision, int maxRevision)
        {
            var result = _client.Search<ElasticSearchCommit>(x =>
                           x.Query(
                               q => q.Term("streamId", streamId.ToString("N")) &&
                                    q.Range(n => n
                                                     .OnField("streamRevision")
                                                     .From(minRevision)
                                                     .To(maxRevision))
                               )).Documents.ToCommits();
            return result;
            //q.Bool(b=>b.Should(r=>r.Range(z=>z.GreaterOrEquals())))//"streamRevision", >= minRevision && x.StartingStreamRevision <= maxRevision")
            //));
            //return new Commit[]{new Commit(Guid.NewGuid(),1,Guid.NewGuid(),1,DateTime.Now,null,null) };
        }

        public void Commit(Commit attempt)
        {
            Logger.Debug(Messages.AttemptingToCommit, attempt.Events.Count, attempt.StreamId, attempt.CommitSequence);

            try
            {
                TryElasticSearch(() =>
                {
                    var commit = attempt.ToElasticSearchCommit(_serializer);
                    if (_client.Get<ElasticSearchCommit>(commit.Id) == null)
                    {
                        _client.Index(commit, _connectionSettings.DefaultIndex);//, new IndexParameters{OpType = OpType.Create});
                        Logger.Debug(Messages.CommitPersisted, attempt.CommitId);
                    }
                    else
                    {
                        throw new ConcurrencyException(string.Format("Concurrency exception for Id: {0}", commit.Id));
                    }
                    

                    return true;
                });
            }
            catch (ConcurrencyException cx)
            {
                Logger.Debug(Messages.ConcurrentWriteDetected);
                Logger.Error(cx.Message);
                throw new ConcurrencyException();
            }
        }

        public Snapshot GetSnapshot(Guid streamId, int maxRevision)
        {
            return null;// new Snapshot(Guid.NewGuid(), 1, null);
        }

        public bool AddSnapshot(Snapshot snapshot)
        {
            return true;
        }

        public IEnumerable<StreamHead> GetStreamsToSnapshot(int maxThreshold)
        {
            return null;// new StreamHead[] { new StreamHead(Guid.NewGuid(), 1, 1) };
        }

        public void Initialize()
        {
            if (Interlocked.Increment(ref _initialized) > 1)
                return;

            Logger.Debug(Messages.InitializingStorage);

            if (!_client.IndexExists(_connectionSettings.DefaultIndex).Exists)
                _client.CreateIndex(_connectionSettings.DefaultIndex, _indexSettings);

            RootObjectMapping mapping = _client.GetMapping<ElasticSearchCommit>(_connectionSettings.DefaultIndex);

            if (mapping == null)
                _client.MapFromAttributes<ElasticSearchCommit>(_connectionSettings.DefaultIndex);
        }

        public IEnumerable<Commit> GetFrom(DateTime start)
        {
            //return new Commit[] { new Commit(Guid.NewGuid(), 1, Guid.NewGuid(), 1, DateTime.Now, null, null) };
            var result = _client.Search<ElasticSearchCommit>(x =>
                           x.Query(
                               q => q.Range(n => n
                                                     .OnField("commitStamp")
                                                     .From(start))
                               )).Documents.ToCommits();
            return result;
        }

        public IEnumerable<Commit> GetFromTo(DateTime start, DateTime end)
        {
            //return new Commit[] { new Commit(Guid.NewGuid(), 1, Guid.NewGuid(), 1, DateTime.Now, null, null) };
            var result = _client.Search<ElasticSearchCommit>(x =>
                           x.Query(
                               q => q.Range(n => n
                                                     .OnField("commitStamp")
                                                     .From(start)
                                                     .To(end))
                               )).Documents.ToCommits();
            return result;
        }

        public IEnumerable<Commit> GetUndispatchedCommits()
        {
            var s = new SearchDescriptor<ElasticSearchCommit>()
               .Query(x=>x.Term("dispatched", "false"))
               .SortAscending(f => f.CommitStamp);
            IQueryResponse<ElasticSearchCommit> response = _client.Search(s);
            
            return response.Total > 0 ? response.Documents.ToCommits() : new List<Commit>();
        }

        public void MarkCommitAsDispatched(Commit commit)
        {
            var elasticSearchCommit = commit.ToElasticSearchCommit(_serializer);
            _client.Update<ElasticSearchCommit>(u => u
                .Object(elasticSearchCommit)
                .Script("ctx._source.dispatched = true")
                .RetriesOnConflict(5)
                .Refresh()
                );
        }

        public void Purge()
        {
            
        }


        protected virtual T TryElasticSearch<T>(Func<T> callback)
        {
            if (_disposed)
                throw new ObjectDisposedException("Attempt to use storage after it has been disposed.");

            try
            {
                return callback();
            }
            catch (ElasticSearchException e)
            {
                Logger.Warn(Messages.StorageThrewException);
                throw new StorageException(e.Message, e);
            }
        }
    }
}
