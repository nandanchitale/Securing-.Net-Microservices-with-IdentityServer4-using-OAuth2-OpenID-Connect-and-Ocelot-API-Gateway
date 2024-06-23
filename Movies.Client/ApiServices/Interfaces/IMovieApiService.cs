using Movie = Movies.Models.Movies;

namespace Movies.Client.ApiServices.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetMovies();
        Task<Movie> GetMovie(string id);
        Task<Movie> CreateMovie(Movie movie);
        Task<Movie> UpdateMovie(Movie movie);
        Task DeleteMovie(int id);
    }
}

