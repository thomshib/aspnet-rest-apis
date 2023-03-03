

public interface IMovieRepository
{

    Task<bool> CreateAsync(Movie movie);

    Task<bool> UpdateAsync(Movie movie);
    Task<bool> DeleteByIdAsync(Guid id);

    Task<Movie?> GetByIdAsync(Guid id);

    Task<Movie?> GetBySlugAsync(string slug);

    Task<IEnumerable<Movie>> GetAllAsync();

      Task<bool> ExistsByIdAsync(Guid id);



}