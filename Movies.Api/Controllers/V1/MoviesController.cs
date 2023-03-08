using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Movies.Api.Controllers.V1;


[ApiController]
[ApiVersion(1.0)]
[ApiVersion(2.0)]

public class MoviesController : ControllerBase
{


    private readonly ILogger<MoviesController> _logger;
    private readonly IMovieService _movieService;

    private readonly IOutputCacheStore _outputCacheStore;

    public MoviesController(IMovieService movieService, ILogger<MoviesController> logger,IOutputCacheStore outputCacheStore)
    {
        _logger = logger;
        _movieService = movieService;
         _outputCacheStore = outputCacheStore;
    }

    //[Authorize(AuthConstants.TrusterMemberPolicyName)]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    [HttpPost(ApiEndpoints.V1.Movies.Create)]
    [ProducesResponseType(typeof(MovieResponse),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse),StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
    {

        var movie = request.MapToMovie();

        var result = await _movieService.CreateAsync(movie, token);

        // evict output cache
       await  _outputCacheStore.EvictByTagAsync("movies", token);

        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
        //return Created($"{ApiEndpoints.Movies.Create}/{movie.Id}", movie);
        //return Ok(movie);      

    }

    
    [HttpGet(ApiEndpoints.V1.Movies.Get)]
    [ProducesResponseType(typeof(MovieResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ResponseCache(Duration = 30,VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
    [OutputCache(PolicyName = "MovieCache")]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {

        var userId = HttpContext.GetUserId();
        var movie = Guid.TryParse(idOrSlug, out var id)
        ? await _movieService.GetByIdAsync(id,userId, token)
        : await _movieService.GetBySlugAsync(idOrSlug,userId, token);

        if (movie is null) return NotFound();

        var response = movie.MapToMovieResponse();
        return Ok(response);


    }

    
    [HttpGet(ApiEndpoints.V1.Movies.GetAll)]
    [ProducesResponseType(typeof(MoviesResponse),StatusCodes.Status200OK)]
    //[ResponseCache(Duration = 30,VaryByQueryKeys = new []{"title","year","sortBy","page","pageSize"}, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
    [OutputCache(PolicyName = "MovieCache")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken token)
    {

        var userId = HttpContext.GetUserId();
        var options = request.MapToOptions().WithUser(userId);
        
        var movies = await _movieService.GetAllAsync(options,token);
        var movieCount = await _movieService.GetCountAsync(options.Title, options.YearOfRelease, token);

        if (movies is null) return NotFound();

        var response = movies.MapToMoviesResponse(request.Page, request.PageSize, movieCount);
        return Ok(response);


    }

    [MapToApiVersion(1.0)]
    [HttpGet(ApiEndpoints.DefaultVersion.Movies.GetAll)]
     [ProducesResponseType(typeof(MoviesResponse),StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllV1([FromQuery] GetAllMoviesRequest request, CancellationToken token)
    {

        var userId = HttpContext.GetUserId();
        var options = request.MapToOptions().WithUser(userId);
        
        var movies = await _movieService.GetAllAsync(options,token);
        var movieCount = await _movieService.GetCountAsync(options.Title, options.YearOfRelease, token);

        if (movies is null) return NotFound();

        var response = movies.MapToMoviesResponse(request.Page, request.PageSize, movieCount);
        return Ok(response);


    }

    [Authorize(AuthConstants.TrusterMemberPolicyName)]
    [ProducesResponseType(typeof(MoviesResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationFailureResponse),StatusCodes.Status400BadRequest)]
    [HttpPut(ApiEndpoints.V1.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
    {

        var movie = request.MapToMovie(id);
        var userId = HttpContext.GetUserId();
        var updatedMovie = await _movieService.UpdateAsync(movie,userId, token);

        if (updatedMovie is null) return NotFound();

        // evict output cache
       await  _outputCacheStore.EvictByTagAsync("movies", token);

        var response = updatedMovie.MapToMovieResponse();

        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.V1.Movies.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {

        var userId = HttpContext.GetUserId();
        var deleted = await _movieService.DeleteByIdAsync(id, token);

        if (!deleted) return NotFound();

         // evict output cache
       await  _outputCacheStore.EvictByTagAsync("movies", token);


        return Ok();
    }

}
