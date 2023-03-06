
using Dapper;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RatingRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;

    }
    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsnc(token);
        return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition(@"
            select round(avg(r.rating),1) from ratings r
            where movieid = @movieId", new { movieId }, cancellationToken: token));
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsnc(token);

        return await connection.QuerySingleOrDefaultAsync<(float?, int)>(new CommandDefinition(@"
             select round(avg(r.rating),1),
             (select rating from ratings where movieid = @moviedId and userid = @userId limit 1)
              from ratings 
              where movieid = @movieId
          
          ", new { movieId, userId }, cancellationToken: token));
    }



    public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken token)
    {
        using var connection = await _connectionFactory.CreateConnectionAsnc(token);

        var result = await connection.ExecuteAsync(new CommandDefinition(@"
         insert into ratings(userid,movieid,rating) values(
            @userId, @movieId,@rating) 
            on conflict(userid,movieid) do update set rating = @rating
       ", new { userId, movieId, rating }, cancellationToken: token));

        return result > 0;
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsnc(token);

        var result = await connection.ExecuteAsync(new CommandDefinition(@"
         delete from ratings where userid = @userid and movieid = @movieId
       ", new { userId, movieId }, cancellationToken: token));

        return result > 0;

    }

    public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {

        using var connection = await _connectionFactory.CreateConnectionAsnc(token);

        return await connection.QueryAsync<MovieRating>(new CommandDefinition(@"
         select r.rating,r.movieid,m.slug 
         from ratings r 
         inner join movies m on r.movieid = m.id
         where r.userid = @userId
       ", new { userId }, cancellationToken: token));


    }

}
