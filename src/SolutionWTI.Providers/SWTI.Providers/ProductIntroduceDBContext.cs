using Microsoft.Extensions.Options;
using SWTI.Configurations;
using SWTI.Interfaces.IProviders;
using System.Data.SqlClient;

namespace SWTI.Providers
{
    public class ProductIntroduceDBContext : IProductIntroduceDBContext
    {
        private readonly string _connectionString;
        public ProductIntroduceDBContext(IOptions<ConnectionStrings> options)
        {
            _connectionString = options.Value.ProductIntroduceConnectionString;
        }

        public SqlConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}