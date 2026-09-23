
using Microsoft.Data.SqlClient;

namespace Envirotrax.LegacyDataMigration.Services;

public class LegacyDbService
{
    private readonly string _connectionString;

    public LegacyDbService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int> ExecuteNonQueryAsync(string sql)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(sql, connection))
            {
                return await command.ExecuteNonQueryAsync();
            }
        }
    }
}