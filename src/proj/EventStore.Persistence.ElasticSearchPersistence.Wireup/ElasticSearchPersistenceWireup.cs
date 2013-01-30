using EventStore.Logging;

namespace EventStore
{
    public class ElasticSearchPersistenceWireup : PersistenceWireup
    {
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(ElasticSearchPersistenceWireup));
        
        public ElasticSearchPersistenceWireup(Wireup inner):base(inner)
        {
            
        }
    }
}
