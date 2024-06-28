using System.Text;
using System.Text.Json;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Movies.API.DTO;
using Movies.Client.ApiServices.Interfaces;
using Movies.DataAccess.IRepository;
using Movies.Models;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MovieApiService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
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
                movieObj = JsonConvert.DeserializeObject<Movie>(content);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception at MovieApiService > UpdateMovie() => {ex.Message}");
            }
            return await Task.FromResult(movie);
        }

        public async Task<UserInfoModel> GetUserInfo()
        {
            UserInfoModel userInfoModel = null;
            try
            {
                HttpClient idpClient = _httpClientFactory.CreateClient(_configuration["IdentityServer:Id"]);
                DiscoveryDocumentResponse discoveryDocumentResponse = await idpClient.GetDiscoveryDocumentAsync();
                if (discoveryDocumentResponse.IsError) throw new HttpRequestException("Something went wrong while requesting Access Token");
                string? accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(
                    OpenIdConnectParameterNames.AccessToken
                );

                UserInfoResponse userInfoResponse = await idpClient.GetUserInfoAsync(
                    new UserInfoRequest
                    {
                        Address = discoveryDocumentResponse.UserInfoEndpoint,
                        Token = accessToken
                    }
                );

                if (userInfoResponse.IsError) throw new HttpRequestException("Something went wrong while requesting User Info");

                Dictionary<string, string> userInfoDict = new Dictionary<string, string>();
                foreach (var claim in userInfoResponse.Claims)
                {
                    userInfoDict.Add(claim.Type, claim.Value);
                }

                userInfoModel = new UserInfoModel(userInfoDict);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception at MovieApiService > GetUserInfo() => {ex.Message}");
            }
            return await Task.FromResult(userInfoModel);
        }
    }
}
