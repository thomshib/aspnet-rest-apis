

using FluentValidation;
using FluentValidation.Results;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IMovieRepository _movieRepository;
    public RatingService(IRatingRepository ratingRepository,IMovieRepository movieRepository)
    {
        _ratingRepository = ratingRepository;
         _movieRepository = movieRepository;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken token = default)
    {
        if (rating is <= 0 or > 5)
        {
            throw new ValidationException(new[]{

                    new ValidationFailure{
                    PropertyName = "Rating",
                    ErrorMessage = "Rating must be between 1 and 5"
                    }
                }
            );
        }

        var movieExists = await _movieRepository.ExistsByIdAsync(movieId, token: token);

        if(!movieExists) return false;

        return await _ratingRepository.RateMovieAsync(movieId,userId,rating,token);

    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default){

        return await _ratingRepository.DeleteRatingAsync(movieId,userId,token);
    }

    public  async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default){
        return await _ratingRepository.GetRatingsForUserAsync(userId,token);
    }
}