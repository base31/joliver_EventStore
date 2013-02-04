using Nest;

namespace EventStore.Persistence.ElasticSearchPersistence
{
    public interface IElasticSearchPersistenceConfiguration
    {
        ConnectionSettings ConnectionSettings { get; }
        IndexSettings IndexSettings { get; }
        ElasticClient ElasticClient { get;  }
    }
}