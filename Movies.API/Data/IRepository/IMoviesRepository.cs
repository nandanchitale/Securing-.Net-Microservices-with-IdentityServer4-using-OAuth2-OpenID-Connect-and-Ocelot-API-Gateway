using Movies.API.Model;

namespace Movies.API.Data.IRepository;

public interface IMoviesRepository
{
    bool SaveChanges();
    IEnumerable<Movie> GetAllMovies();

    Movie GetMovieById(int id);

    IEnumerable<Movie> GetAllMatchingMovies(string movieName);

    void CreateMovie(Movie movie);
    (bool, Movie) isMovieAlreadyExists(string movieName);
}