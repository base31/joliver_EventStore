using EventStore.Persistence.ElasticSearchPersistence;
using EventStore.Serialization;

namespace EventStore
{
    public static class ElasticSearchPersistenceWireupExtensions
    {
        public static ElasticSearchPersistenceWireup UsingElasticSearchPersistence(this Wireup inner,
                                                                      IElasticSearchPersistenceConfiguration configuration,
                                                                      IDocumentSerializer documentSerializer)
        {
            return new ElasticSearchPersistenceWireup(inner, configuration, documentSerializer);
        }
    }
}