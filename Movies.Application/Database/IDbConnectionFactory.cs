using System.Data;
using Npgsql;

public interface IDbConnectionFactory
{

    Task<IDbConnection> CreateConnectionAsnc();

}


public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    public async Task<IDbConnection> CreateConnectionAsnc()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
        
    }
}