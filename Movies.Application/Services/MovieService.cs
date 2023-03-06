
using FluentValidation;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    private readonly IRatingRepository _ratingRepository;
    private readonly IValidator<Movie> _movieValidator;

    public MovieService(IMovieRepository repository,IRatingRepository ratingRepository,IValidator<Movie> movieValidator)
    {
         _ratingRepository = ratingRepository;
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

  

    public Task<IEnumerable<Movie>> GetAllAsync(Guid? userId = default,CancellationToken token = default)
    {
         return  _movieRepository.GetAllAsync(userId,token);
    }

    public Task<Movie?> GetByIdAsync(Guid id,Guid? userId = default,CancellationToken token = default)
    {
         return  _movieRepository.GetByIdAsync(id,userId,token);
    }

    public Task<Movie?> GetBySlugAsync(string slug,Guid? userId = default,CancellationToken token = default)
    {
        return    _movieRepository.GetBySlugAsync(slug,userId,token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie,Guid? userId = default,CancellationToken token = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, token);
        var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id,token);

        if(!movieExists) return null;

        await _movieRepository.UpdateAsync(movie,token);

        if(!userId.HasValue){
            var rating = await _ratingRepository.GetRatingAsync(movie.Id, token);
            movie.Rating = rating;
            return movie;
        }

         var ratings = await _ratingRepository.GetRatingAsync(movie.Id,userId.Value, token);
         movie.Rating = ratings.Rating;
         movie.UserRating = ratings.UserRating;


        return movie;

    }
}