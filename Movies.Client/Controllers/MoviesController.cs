using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Movies.Client.ApiServices.Interfaces;
using Movies.DataAccess;
using Movies.DataAccess.IRepository;
using Movies.Utils.Constants;
using Movie = Movies.Models.Movies;

namespace Movies.Client.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        IMoviesRepository _repository;
        AppDbContext _context;
        IMovieService _movieService;

        public MoviesController(AppDbContext dbContext, IMoviesRepository repository, IMovieService movieService)
        {
            _repository = repository;
            _context = dbContext;
            _movieService = movieService;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            IActionResult response = View(NoContent());
            try
            {
                LogTokenAndClaims();
                IEnumerable<Movie> movies = await _movieService.GetMovies(); //_repository.GetAllMovies();
                response = View(movies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Exception at MoviesController > Index : {ex.Message}");
                response = View(BadRequest(ex.Message));
            }
            return response;
        }

        // GET: Movies/Details/5
        public IActionResult Details(int? id)
        {
            IActionResult response = NotFound();
            try
            {
                if (id is null)
                    return NotFound();

                Movie movie = _repository.GetMovieById((int)id);
                if (movie is not null)
                    response = View(movie);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Exception at MoviesController > Details : {ex.Message}");
                response = BadRequest(ex.Message);
            }
            return response;
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Title,Genre,ReleaseDate,Owner")] Movie movie)
        {
            IActionResult response = BadRequest();
            try
            {
                if (ModelState.IsValid)
                {
                    _repository.CreateMovie(movie);
                    _repository.SaveChanges();
                    response = RedirectToAction(nameof(Index));
                }
                response = View(movie);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Exception at MoviesController > Create : {ex.Message}");
                response = View(BadRequest(ex.Message));
            }
            return response;
        }

        // GET: Movies/Edit/5
        public IActionResult Edit(int? id)
        {
            IActionResult response = NotFound();
            try
            {
                if (id is null)
                    return response;

                Movie movie = _repository.GetMovieById((int)id);
                if (movie is not null)
                    response = View(movie);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Exception at MoviesController > Edit : {ex.Message}");
                response = View(BadRequest(ex.Message));
            }
            return response;
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Title,Genre,ReleaseDate,Owner")] Movie movie
        )
        {
            IActionResult response = View(movie);
            try
            {
                if (id != movie.Id)
                    return NotFound();
                if (!ModelState.IsValid)
                    return View(movie);
                _context.Update(movie);
                _context.SaveChanges();
                response = RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Exception at MoviesController > Edit : {ex.Message}");
                response = View(BadRequest(ex.Message));
            }
            return response;
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            IActionResult response = NotFound();
            try
            {
                if (id == null)
                    return response;

                Movie movie = _repository.GetMovieById((int)id);

                if (movie == null)
                    return response;

                response = View(movie);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Exception at MoviesController > Delete : {ex.Message}");
                response = View(BadRequest(ex.Message));
            }
            return response;
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            IActionResult response = NotFound();
            try
            {
                Movie movie = _repository.GetMovieById(id);
                movie.Status = Status.INACTIVE;
                _repository.SaveChanges();
                response = RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Exception at MoviesController > Delete : {ex.Message}");
                response = View(BadRequest(ex.Message));
            }
            return response;
        }

        public async Task Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Exception at MoviesController > Logout : {ex.Message}");
            }
        }

        private async void LogTokenAndClaims()
        {
            try
            {
                string? identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
                Debug.WriteLine($"--> Identity Token : {identityToken}");
                Debug.WriteLine("--> Claim Type | Claim Value");
                foreach (Claim claim in User.Claims)
                {
                    Debug.WriteLine($"{claim.Type} | {claim.Value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Exception at MoviesController > LogTokenAndClaims : {ex.Message}");
            }
        }
    }
}

