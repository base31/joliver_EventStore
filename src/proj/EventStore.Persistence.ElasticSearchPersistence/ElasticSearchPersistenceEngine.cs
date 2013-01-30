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
            throw new NotImplementedException();
        }

        public void Commit(Commit attempt)
        {
            Logger.Debug(Messages.AttemptingToCommit, attempt.Events.Count, attempt.StreamId, attempt.CommitSequence);


            try
            {
                TryElasticSearch(() =>
                {
                    var commit = attempt.ToElasticSearchCommit(_serializer);           
                    _client.Index(commit, _connectionSettings.DefaultIndex, new IndexParameters{OpType = OpType.Create});
                    Logger.Debug(Messages.CommitPersisted, attempt.CommitId);

                    return true;
                });
            }
            catch (ConcurrencyException cx)
            {
                Logger.Debug(Messages.ConcurrentWriteDetected);
                Logger.Error(cx.Message);
            }
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
