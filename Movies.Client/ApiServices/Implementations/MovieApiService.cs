using System.Text;
using System.Text.Json;
using IdentityModel.Client;
using Movies.API.DTO;
using Movies.Client.ApiServices.Interfaces;
using Movies.DataAccess.IRepository;
using Movies.Utils.Constants;
using Newtonsoft.Json;
using Movie = Movies.Models.Movies;

namespace Movies.Client.ApiServices.Implementation
{
    public class MovieApiService : IMovieService
    {
        private readonly IConfiguration _configuration;
        private HttpClient httpClient;
        private HttpResponseMessage response;
        HttpRequestMessage request;
        private readonly IHttpClientFactory _httpClientFactory;
        public MovieApiService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            httpClient = _httpClientFactory.CreateClient(_configuration["IdentityServer:ClientId"]);
            httpClient.BaseAddress = new Uri(_configuration["MoviesAPI"]);
        }

        public async Task<Movie> CreateMovie(MovieDetailDto movie)
        {
            Movie movieObj = null;
            try
            {
                string requestBody = JsonConvert.SerializeObject(movie);
                StringContent requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                response = await httpClient.PostAsync(string.Empty, requestContent);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"--> MovieApiService -> CreateMovie : ${content}");
                movieObj = JsonConvert.DeserializeObject<Movie>(content);
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
                string uri = Path.Combine(_configuration["MoviesAPI"], id.ToString());
                response = await httpClient.DeleteAsync(uri);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception at MovieApiService > DeleteMovie() => {ex.Message}");
            }
        }

        public async Task<Movie> GetMovie(string id)
        {
            Movie movie = null;
            try
            {
                string uri = Path.Combine(_configuration["MoviesAPI"], id.ToString());
                response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                movie = JsonConvert.DeserializeObject<Movie>(content);
                Console.WriteLine($"--> MovieApiService -> GetMovie : ${content}");

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
                request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
                response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                movies = JsonConvert.DeserializeObject<List<Movie>>(content);
                Console.WriteLine($"--> MovieApiService -> GetMovies : ${content}");
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
                string requestBody = JsonConvert.SerializeObject(movie);
                StringContent requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                response = await httpClient.PutAsync(movie.Id.ToString(), requestContent);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"--> MovieApiService -> UpdateMovie : ${content}");
                movieObj = JsonConvert.DeserializeObject<Movie>(content);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception at MovieApiService > UpdateMovie() => {ex.Message}");
            }
            return await Task.FromResult(movie);
        }
    }
}
