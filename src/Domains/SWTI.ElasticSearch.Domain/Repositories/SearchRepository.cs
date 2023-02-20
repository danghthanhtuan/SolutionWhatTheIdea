using EasyNetQ.Logging;
using SWTI.Interfaces.IProviders;
using SWTI.Interfaces.IRepositories.ElasticSearch;

namespace SWTI.ElasticSearch.Domain.Repositories
{
    public class SearchRepository : ISearchRepository
    {
        private readonly ILogger<SearchRepository> _logger;
        private readonly IElasticSearchContext _elasticContext;
        public SearchRepository(IElasticSearchContext elasticContext
         , ILogger<SearchRepository> logger)
        {
            _logger = logger;
            _elasticContext = elasticContext;
        }
    }
}
