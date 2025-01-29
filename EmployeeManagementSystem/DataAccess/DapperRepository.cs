using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace EmployeeManagementSystem.DataAccess
{
    public class DapperRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection Connection => new SqlConnection(_connectionString);

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object parameters = null)
        {
            using (var connection = Connection)
            {
                return await connection.QueryAsync<T>(query, parameters);
            }
        }

        public async Task<int> ExecuteAsync(string query, object parameters = null)
        {
            using (var connection = Connection)
            {
                return await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
