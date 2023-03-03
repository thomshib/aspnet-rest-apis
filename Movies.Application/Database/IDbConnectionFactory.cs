using System.Data;
using Npgsql;

public interface IDbConnectionFactory
{

    Task<IDbConnection> CreateConnectionAsnc(CancellationToken token = default);

}


public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    public async Task<IDbConnection> CreateConnectionAsnc(CancellationToken token = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);
        return connection;
        
    }
}