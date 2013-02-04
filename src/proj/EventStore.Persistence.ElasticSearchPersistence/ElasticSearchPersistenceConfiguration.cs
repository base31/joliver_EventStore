using Nest;

namespace EventStore.Persistence.ElasticSearchPersistence
{
    public class ElasticSearchPersistenceConfiguration : IElasticSearchPersistenceConfiguration
    {
        public ConnectionSettings ConnectionSettings { get; private set; }
        public IndexSettings IndexSettings { get; private set; }
        public ElasticClient ElasticClient { get; private set; }

        public ElasticSearchPersistenceConfiguration(string host, int port, string defaultIndex, IndexSettings indexSettings)
        {
            ConnectionSettings = new ConnectionSettings(host, port);
            ConnectionSettings.SetDefaultIndex(defaultIndex);
            IndexSettings = indexSettings;

            ElasticClient = new ElasticClient(ConnectionSettings);
        }
    }
}