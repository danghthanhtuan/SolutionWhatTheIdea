using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using SWTI.Configurations;
using SWTI.Interfaces.IProviders;

namespace SWTI.Providers
{
    public class ElasticSearchContext : IElasticSearchContext
    {
        private readonly ElasticLowLevelClient _elasticClient;

        public ElasticSearchContext(IOptions<ConnectionStrings> options)
        {
            var settings = new ConnectionConfiguration(new Uri(options.Value.ES_Transaction)).RequestTimeout(TimeSpan.FromMinutes(2));
#if DEBUG
            settings.DisableDirectStreaming();
#endif
            _elasticClient = new ElasticLowLevelClient(settings);
        }

        public ElasticLowLevelClient ElasticClient { get { return _elasticClient; } }
    }
}
