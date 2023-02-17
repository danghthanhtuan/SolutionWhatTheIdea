using System.Data.SqlClient;

namespace SWTI.Interfaces.IProviders
{
    public interface IProductIntroduceDBContext
    {
        SqlConnection CreateConnection();
    }
}
