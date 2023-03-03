
using FluentValidation;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IValidator<Movie> _movieValidator;

    public MovieService(IMovieRepository repository,IValidator<Movie> movieValidator)
    {
        _movieRepository = repository;
        _movieValidator = movieValidator;
    }
    public async Task<bool> CreateAsync(Movie movie,CancellationToken token = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie,token);
        return  await _movieRepository.CreateAsync(movie,token);
    }

    public Task<bool> DeleteByIdAsync(Guid id,CancellationToken token = default)
    {
        return _movieRepository.DeleteByIdAsync(id,token);
    }

  

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
    {
         return  _movieRepository.GetAllAsync(token);
    }

    public Task<Movie?> GetByIdAsync(Guid id,CancellationToken token = default)
    {
         return  _movieRepository.GetByIdAsync(id,token);
    }

    public Task<Movie?> GetBySlugAsync(string slug,CancellationToken token = default)
    {
        return    _movieRepository.GetBySlugAsync(slug,token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie,CancellationToken token = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, token);
        var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id,token);

        if(!movieExists) return null;

        await _movieRepository.UpdateAsync(movie,token);
        return movie;

    }
}