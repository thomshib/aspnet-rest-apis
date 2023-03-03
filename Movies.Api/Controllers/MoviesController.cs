using Microsoft.AspNetCore.Mvc;

namespace Movies.Api.Controllers;

[ApiController]

public class MoviesController : ControllerBase
{
   

    private readonly ILogger<MoviesController> _logger;
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService, ILogger<MoviesController> logger)
    {
        _logger = logger;
        _movieService = movieService;
    }

    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request){

        var movie = request.MapToMovie();

        var result = await _movieService.CreateAsync(movie);

        return CreatedAtAction(nameof(Get),new {idOrSlug = movie.Id}, movie);
        //return Created($"{ApiEndpoints.Movies.Create}/{movie.Id}", movie);
        //return Ok(movie);      

    }

     [HttpGet(ApiEndpoints.Movies.Get)]
     public async Task<IActionResult> Get([FromRoute] string idOrSlug){

        var movie = Guid.TryParse(idOrSlug, out var id) 
        ? await _movieService.GetByIdAsync(id)
        : await _movieService.GetBySlugAsync(idOrSlug);

        if(movie is null) return NotFound();

        var response = movie.MapToMovieResponse();
        return Ok(response);


     }

     
     [HttpGet(ApiEndpoints.Movies.GetAll)]
     public async Task<IActionResult> GetAll(){

        var movies = await _movieService.GetAllAsync();

        if(movies is null) return NotFound();

        var response = movies.MapToMoviesResponse();
        return Ok(response);


     }

     [HttpPut(ApiEndpoints.Movies.Update)] 
     public async Task<IActionResult> Update([FromRoute] Guid id , [FromBody] UpdateMovieRequest request ){

        var movie = request.MapToMovie(id);
        var updatedMovie = await _movieService.UpdateAsync(movie);

        if(updatedMovie is null) return NotFound();

        var response = updatedMovie.MapToMovieResponse();

        return Ok(response);
     }

      [HttpDelete(ApiEndpoints.Movies.Delete)] 
     public async Task<IActionResult> Delete  ([FromRoute] Guid id ){

       
        var deleted = await _movieService.DeleteByIdAsync(id);

        if(!deleted) return NotFound();  

        return Ok();
     }

}
