using IdentityModel.Client;
using Movies.Client.ApiServices.Interfaces;
using Movies.DataAccess.IRepository;
using Movies.Utils.Constants;
using Newtonsoft.Json;
using Movie = Movies.Models.Movies;

namespace Movies.Client.ApiServices.Implementation
{
    public class MovieApiService : IMovieService
    {
        private IApiHandlerService _apiHandlerService;
        private readonly IConfiguration _configuration;
        private HttpClient httpClient;
        private HttpResponseMessage response;
        HttpRequestMessage request;
        public MovieApiService(IConfiguration configuration, IApiHandlerService apiHandlerService)
        {
            _configuration = configuration;
            _apiHandlerService = apiHandlerService ?? throw new ArgumentNullException(nameof(apiHandlerService));
            httpClient = _apiHandlerService.CreateHttpClient(_configuration["IdentityServer:ClientId"]);
        }

        public async Task<Movie> CreateMovie(Movie movie)
        {
            Movie movieObj = null;
            try
            {
                request = _apiHandlerService.PrepareRequestMessage(
                   HttpMethod.Post,
                   "/api/movies/"
               );

                response = await _apiHandlerService.GetApiResponse(false);

                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                // _repository.CreateMovie(movie);
                // _repository.SaveChanges();
                // (bool movieStatus, movieObj) = _repository.isMovieAlreadyExists(movie.Title);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception at MovieApiService > CreateMovie() => {ex.Message}");
            }
            return await Task.FromResult(movieObj);
        }

        public async Task DeleteMovie(int id)
        {
            try
            {
                request = _apiHandlerService.PrepareRequestMessage(
                   HttpMethod.Post,
                   "/api/movies/"
               );

                response = await _apiHandlerService.GetApiResponse(false);

                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                // Movie movie = _repository.GetMovieById(id);
                // if (movie is not null)
                // {
                //     movie.Status = Status.INACTIVE;

                //     _repository.SaveChanges();
                // }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception at MovieApiService > DeleteMovie() => {ex.Message}");
            }
            // return Task.FromResult(Task.CompletedTask);
        }

        public async Task<Movie> GetMovie(string id)
        {
            Movie movie = null;
            try
            {
                request = _apiHandlerService.PrepareRequestMessage(
                    HttpMethod.Post,
                    "/api/movies/id"
                );

                response = await _apiHandlerService.GetApiResponse(false);

                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                movie = JsonConvert.DeserializeObject<Movie>(content);
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
                request = _apiHandlerService.PrepareRequestMessage(
                    HttpMethod.Get,
                    "/api/movies/"
                );

                response = await _apiHandlerService.GetApiResponse(false);

                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                // Deserialize object to movies list
                movies = JsonConvert.DeserializeObject<List<Movie>>(content);

                /******************************************* Method 2 *******************************************
                // // 1> Get API Credentials. This must be registered on Identity Server
                // ClientCredentialsTokenRequest apiClientCredentials = new ClientCredentialsTokenRequest
                // {
                //     Address = $"{_configuration["IdentityServer:Host"]}/connect/token",
                //     ClientId = _configuration["IdentityServer:ClientId"],
                //     ClientSecret = _configuration["IdentityServer:ClientSecret"],
                //     Scope = _configuration["IdentityServer:Scope"],
                // };

                // // Creates a new HttpClient to talsk to our identity server
                // HttpClient client = new HttpClient();

                // // Check whether we can reach discovery document. (Optional)
                // DiscoveryDocumentResponse? disc = await client.GetDiscoveryDocumentAsync(_configuration["IdentityServer:Host"]);
                // if (disc.IsError) return null;

                // // Authenticate and Get an access token from Identity Server
                // Task<TokenResponse>? tokenResponse = client.RequestClientCredentialsTokenAsync(apiClientCredentials);
                // if (tokenResponse.IsFaulted) return null;

                // // Send Request to Protected API
                // // Another HttpClient to talk with protected api
                // HttpClient apiClient = new HttpClient();
                // apiClient.SetBearerToken(tokenResponse.Result.AccessToken);

                // // Send request to protected api
                // HttpResponseMessage response = await apiClient.GetAsync($"{_configuration["MoviesAPI"]}/api/movies");
                // response.EnsureSuccessStatusCode();

                // string content = await response.Content.ReadAsStringAsync();

                // Deserialize object to movies list
                movies = JsonConvert.DeserializeObject<List<Movie>>(content);

                *******************************************
                // movies = _repository.GetAllMovies();
                ******************************************* Method 2 *******************************************/
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception at MovieApiService > GetMovies() => {ex.Message}");
            }
            return await Task.FromResult(movies);
        }

        public async Task<Movie> UpdateMovie(Movie movie)
        {
            Movie movieObj = null;
            try
            {
                request = _apiHandlerService.PrepareRequestMessage(
                    HttpMethod.Get,
                    "/api/movies/"
                );

                response = await _apiHandlerService.GetApiResponse(false);

                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                movieObj = JsonConvert.DeserializeObject<Movie>(content);
                // (bool movieStatus, movieObj) = _repository.isMovieAlreadyExists(movie.Title);

                // if (movieObj is null)
                //     return null;
                // else if (!movieStatus)
                // {
                //     movie.Title = movie.Title;
                //     movie.Owner = movie.Owner;
                //     movie.ReleaseDate = movie.ReleaseDate;
                //     movie.Status = Status.ACTIVE;

                //     _repository.SaveChanges();
                // }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception at MovieApiService > GetMovies() => {ex.Message}");
            }
            return await Task.FromResult(movie);
        }
    }
}
