using Elasticsearch.Net;

namespace SWTI.Interfaces.IProviders
{
    public interface IElasticSearchContext
    {
        ElasticLowLevelClient ElasticClient { get; }
    }
}
