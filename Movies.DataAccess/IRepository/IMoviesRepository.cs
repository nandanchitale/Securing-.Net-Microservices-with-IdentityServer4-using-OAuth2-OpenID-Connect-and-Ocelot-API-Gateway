using Movie = Movies.Models.Movies;

namespace Movies.DataAccess.IRepository;

public interface IMoviesRepository
{
    bool SaveChanges();
    IEnumerable<Movie> GetAllMovies();

    Movie GetMovieById(int id);

    IEnumerable<Movie> GetAllMatchingMovies(string movieName);

    void CreateMovie(Movie movie);

    (bool, Movie) isMovieAlreadyExists(string movieName);
}

