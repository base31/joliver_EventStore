using System.Transactions;
using EventStore.Logging;
using EventStore.Persistence.ElasticSearchPersistence;
using EventStore.Serialization;

namespace EventStore
{
    public class ElasticSearchPersistenceWireup : PersistenceWireup
    {
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof (ElasticSearchPersistenceWireup));

        public ElasticSearchPersistenceWireup(Wireup inner, IElasticSearchPersistenceConfiguration configuration,
                                              IDocumentSerializer documentSerializer)
            : base(inner)
        {
            Logger.Debug("Configuring ElasticSearch persistence engine.");

            var options = Container.Resolve<TransactionScopeOption>();
            if (options != TransactionScopeOption.Suppress)
                Logger.Warn("ElasticSearch does not participate in transactions using TransactionScope.");

            Container.Register(c => new ElasticSearchPersistenceFactory(
                                        configuration,
                                        documentSerializer).Build());
        }
    }
}