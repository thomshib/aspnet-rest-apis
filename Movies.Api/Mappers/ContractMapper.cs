

public static class ContractMapper
{

    public static Movie MapToMovie(this CreateMovieRequest request)
    {
        return new Movie
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }

    public static MovieResponse MapToMovieResponse(this Movie movie)
    {

        return new MovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            YearOfRelease = movie.YearOfRelease,
            UserRating = movie.UserRating,
            Rating = movie.Rating,
            Genres = movie.Genres,
            Slug = movie.Slug

        };
    }

    public static MoviesResponse MapToMoviesResponse(this IEnumerable<Movie> movies, 
    int page, int pageSize, int totalCount)
    {
        return new MoviesResponse{
            Items = movies.Select(MapToMovieResponse),
            Page = page,
            PageSize = pageSize,
            Total = totalCount
        };
    }

    public static Movie MapToMovie(this UpdateMovieRequest request, Guid id){

        return new Movie
        {
            Id = id,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };

    }

     public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> ratings)
    {

        return ratings.Select( x => new MovieRatingResponse
        {
            Rating = x.Rating,
            Slug = x.Slug,
            MovieId = x.MovieId
        });

    }

    public static GetAllMoviesOptions MapToOptions( this GetAllMoviesRequest request){

        return new GetAllMoviesOptions{
            Title = request.Title,
            YearOfRelease = request.Year,
            SortField = request.SortBy?.Trim('+','-'),
            SortOrder = request.SortBy is null ? SortOrder.Unsorted :
                request.SortBy.StartsWith('-') ? SortOrder.Descending  : SortOrder.Ascending,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public static GetAllMoviesOptions WithUser(this GetAllMoviesOptions options, Guid? userId ){
        options.UserId = userId;
        return options;
    } 
}
