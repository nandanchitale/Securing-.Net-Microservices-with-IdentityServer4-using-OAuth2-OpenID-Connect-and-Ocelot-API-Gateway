using Movies.API.DTO;
using Movies.Models;
using Movie = Movies.Models.Movies;

namespace Movies.Client.ApiServices.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetMovies();
        Task<Movie> GetMovie(string id);
        Task<Movie> CreateMovie(MovieDetailDto movie);
        Task<Movie> UpdateMovie(Movie movie);
        Task DeleteMovie(int id);
        Task<UserInfoModel> GetUserInfo();
    }
}

