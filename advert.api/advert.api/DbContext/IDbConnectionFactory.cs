using System.Data;

namespace advert.api.DbContext
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
