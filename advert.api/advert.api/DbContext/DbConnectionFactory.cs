using System.Data;
using System.Data.SqlClient;
namespace advert.api.DbContext
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;


        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;

        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
