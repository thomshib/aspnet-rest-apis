using FluentValidation;

public class MovieValdiator : AbstractValidator<Movie>
{
    private IMovieRepository _movieRepository;
    public MovieValdiator(IMovieRepository movieRepository){
    
        _movieRepository =  movieRepository;
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Genres).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Genres).NotEmpty();
        RuleFor(x => x.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year);
        RuleFor(x => x.Slug).MustAsync(ValidateSlug).WithMessage("This movie already exists");

    }

    private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken token = default)
    {
        var existingMovie = await _movieRepository.GetBySlugAsync(slug);

        if(existingMovie is not null) return existingMovie.Id == movie.Id;

        return existingMovie is null;

    }
}