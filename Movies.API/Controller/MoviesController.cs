using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Movies.API.Data;
using Movies.API.Data.IRepository;
using Movies.API.Data.Repository;
using Movies.API.DTO;
using Movies.API.Model;

namespace Movies.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMoviesRepository _repository;
    private readonly IMapper _mapper;

    public MoviesController(
        IMoviesRepository moviesRepository,
        IMapper mapper
    )
    {
        _repository = moviesRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult Get()
    {
        IActionResult response = NoContent();
        try
        {
            IEnumerable<Movie> movies = _repository.GetAllMovies();
            response = Ok(movies);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Exception at MoviesController > Get : {ex.Message}");
            response = BadRequest(ex.Message);
        }
        return response;
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        IActionResult response = NotFound();
        try
        {
            Movie movie = _repository.GetMovieById(id);
            if(movie is not null) response = Ok(movie);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Exception at MoviesController > Get : {ex.Message}");
            response = BadRequest(ex.Message);
        }
        return response;
    }

    [HttpPut]
    public async Task<IActionResult> PutMovie(MovieCreateDto movieCreateDto)
    {
        IActionResult response = NoContent();
        try
        {
            Movie movie = _mapper.Map<Movie>(movieCreateDto);
            _repository.CreateMovie(movie);
            _repository.SaveChanges();

            MovieReadDto movieReadDto = _mapper.Map<MovieReadDto>(movie);
            response = CreatedAtRoute(
                nameof(Get),
                new { id = movieReadDto.Id },
                movieReadDto
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Exception at MoviesController > Get : {ex.Message}");
            response = BadRequest(ex.Message);
        }
        return response;
    }
}