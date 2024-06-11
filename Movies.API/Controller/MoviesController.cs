using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.API.Data;
using Movies.API.Data.IRepository;
using Movies.API.Data.Repository;
using Movies.API.DTO;
using Movies.API.Model;
using Movies.API.Utils.Constants;

namespace Movies.API.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize("ClientIdPolicy")]
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

    [HttpGet(Name = "GetAllMovies")]
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

    [HttpGet("{id}", Name = "GetMovieById")]
    public IActionResult GetMovieById(int id)
    {
        IActionResult response = NotFound();
        try
        {
            Movie movie = _repository.GetMovieById(id);
            if (movie is not null) response = Ok(movie);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Exception at MoviesController > GetMovieById : {ex.Message}");
            response = BadRequest(ex.Message);
        }
        return response;
    }


    [HttpPut("{id}")]
    public IActionResult PutMovie(int id, MovieDetailDto movieDetailDto)
    {
        IActionResult response = NotFound();
        try
        {
            Movie movie = _repository.GetMovieById(id);
            if (movie is not null)
            {
                movie = UpdateMovieData(movie, movieDetailDto);
                if (movie is not null)
                {

                    MovieReadDto movieReadDto = _mapper.Map<MovieReadDto>(movie);
                    response = CreatedAtRoute(
                        nameof(GetMovieById),
                        new { id = id },
                        movieReadDto
                    );
                }
                else response = BadRequest();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Exception at MoviesController > PutMovie : {ex.Message}");
            response = BadRequest(ex.Message);
        }
        return response;
    }

    [HttpPost]
    public IActionResult PostMovie(MovieDetailDto movieCreateDto)
    {
        IActionResult response = NoContent();
        try
        {
            (bool movieStatus, Movie movie) = _repository.isMovieAlreadyExists(movieCreateDto.Title);

            if (movie is null)
            {

                movie = _mapper.Map<Movie>(movieCreateDto);
                _repository.CreateMovie(movie);

                MovieReadDto movieReadDto = _mapper.Map<MovieReadDto>(movie);
                response = CreatedAtRoute(
                    nameof(GetMovieById),
                    new { id = movieReadDto.Id },
                    movieReadDto
                );
            }
            else if (!movieStatus)
            {
                movie = UpdateMovieData(movie, movieCreateDto);

                if (movie is not null)
                {
                    MovieReadDto movieReadDto = _mapper.Map<MovieReadDto>(movie);
                    response = CreatedAtRoute(
                        nameof(GetMovieById),
                        new { id = movieReadDto.Id },
                        movieReadDto
                    );
                }
                else response = BadRequest();
            }
            else response = Conflict($"Movie with name {movieCreateDto.Title} already present");

            _repository.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Exception at MoviesController > PostMovie : {ex.Message}");
            response = BadRequest(ex.Message);
        }
        return response;
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMovie(int id)
    {
        IActionResult response = NotFound();
        try
        {
            Movie movie = _repository.GetMovieById(id);
            if (movie is not null)
            {

                movie.Status = Status.INACTIVE;
                _repository.SaveChanges();

                response = Accepted();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Exception at MoviesController > DeleteMovie : {ex.Message}");
            response = BadRequest(ex.Message);
        }
        return response;
    }

    [NonAction]
    private Movie UpdateMovieData(Movie movie, MovieDetailDto movieDetailDto)
    {
        Movie returnValue = null;
        try
        {
            movie.Title = movieDetailDto.Title;
            movie.Owner = movieDetailDto.Owner;
            movie.ReleaseDate = movieDetailDto.ReleaseDate;
            movie.Status = Status.ACTIVE;

            returnValue = movie; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Exception at MoviesController > UpdateMovieData : {ex.Message}");
        }
        return returnValue;
    }
}