using EventStore.Serialization;
using Nest;

namespace EventStore.Persistence.ElasticSearchPersistence
{
    public class ElasticSearchPersistenceFactory : IPersistenceFactory
    {
        private readonly IElasticSearchPersistenceConfiguration _configuration;
        private readonly IDocumentSerializer _documentSerializer;

        public ElasticSearchPersistenceFactory(IElasticSearchPersistenceConfiguration configuration, IDocumentSerializer documentSerializer)
        {
            _configuration = configuration;
            _documentSerializer = documentSerializer;
        }

        public IPersistStreams Build()
        {
            return new ElasticSearchPersistenceEngine(_configuration, _documentSerializer);
        }
    }
}