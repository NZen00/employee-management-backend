// Repositories/BaseRepository.cs
using employee_management_backend.Data;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace employee_management_backend.Repositories;
public abstract class BaseRepository
{
    protected readonly IDbConnectionFactory _connectionFactory;

    protected BaseRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    protected async Task<int> ExecuteNonQueryAsync(string sql, params SqlParameter[] parameters)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync() as SqlConnection;
        using var command = new SqlCommand(sql, connection);

        if (parameters?.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        return await command.ExecuteNonQueryAsync();
    }

    protected async Task<T> ExecuteReaderAsync<T>(string sql, Func<SqlDataReader, T> mapper, params SqlParameter[] parameters)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync() as SqlConnection;
        using var command = new SqlCommand(sql, connection);

        if (parameters?.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return mapper(reader);
        }

        return default;
    }

    protected async Task<List<T>> ExecuteReaderListAsync<T>(string sql, Func<SqlDataReader, T> mapper, params SqlParameter[] parameters)
    {
        var results = new List<T>();

        using var connection = await _connectionFactory.CreateConnectionAsync() as SqlConnection;
        using var command = new SqlCommand(sql, connection);

        if (parameters?.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(mapper(reader));
        }

        return results;
    }
}