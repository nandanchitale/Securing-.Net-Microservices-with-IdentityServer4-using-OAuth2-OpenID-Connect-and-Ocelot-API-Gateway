using Movies.Client.ApiServices.Interfaces;
using Movies.DataAccess.IRepository;
using Movies.Utils.Constants;
using Movie = Movies.Models.Movies;

namespace Movies.Client.ApiServices.Implementation
{
    public class MovieApiService : IMovieService
    {
        private readonly IMoviesRepository _repository;

        public MovieApiService(IMoviesRepository repository)
        {
            _repository = repository;
        }

        public async Task<Movie> CreateMovie(Movie movie)
        {
            Movie movieObj = null;
            try
            {
                _repository.CreateMovie(movie);
                _repository.SaveChanges();
                (bool movieStatus, movieObj) = _repository.isMovieAlreadyExists(movie.Title);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception at MovieApiService > CreateMovie() => {ex.Message}");
            }
            return await Task.FromResult(movieObj);
        }

        public Task DeleteMovie(int id)
        {
            try
            {
                Movie movie = _repository.GetMovieById(id);
                if (movie is not null)
                {
                    movie.Status = Status.INACTIVE;

                    _repository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception at MovieApiService > DeleteMovie() => {ex.Message}");
            }
            return Task.FromResult(Task.CompletedTask);
        }

        public async Task<Movie> GetMovie(string id)
        {
            Movie movie = null;
            try
            {
                movie = _repository.GetMovieById(int.Parse(id));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception at MovieApiService > GetMovie() => {ex.Message}");
            }
            return await Task.FromResult(movie);
        }

        public async Task<IEnumerable<Movie>> GetMovies()
        {
            IEnumerable<Movie> movies = null;
            try
            {
                movies = _repository.GetAllMovies();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception at MovieApiService > GetMovies() => {ex.Message}");
            }
            return await Task.FromResult(movies);
        }

        public Task<Movie> UpdateMovie(Movie movie)
        {
            Movie movieObj = null;
            try
            {
                (bool movieStatus, movieObj) = _repository.isMovieAlreadyExists(movie.Title);

                if (movieObj is null)
                    return null;
                else if (!movieStatus)
                {
                    movie.Title = movie.Title;
                    movie.Owner = movie.Owner;
                    movie.ReleaseDate = movie.ReleaseDate;
                    movie.Status = Status.ACTIVE;

                    _repository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception at MovieApiService > GetMovies() => {ex.Message}");
            }
            return Task.FromResult(movie);
        }
    }
}
