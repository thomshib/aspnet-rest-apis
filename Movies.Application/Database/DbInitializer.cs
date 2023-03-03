using Dapper;

public class DbInitializer{
    private readonly IDbConnectionFactory _connectionFactory;
    public DbInitializer( IDbConnectionFactory connectionFactory)
    {
          _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync(){
        using var connection = await _connectionFactory.CreateConnectionAsnc();
        await connection.ExecuteAsync(@"
            create table  if not exists movies(
                id UUID primary key,
                slug TEXT not null,
                title TEXT not null,
                yearofrelease integer not null
                );        
        ");

         await connection.ExecuteAsync(@"
            create table  if not exists genres(
                movieid UUID references movies(id),
                name TEXT not null              
                );        
        ");
    }
}