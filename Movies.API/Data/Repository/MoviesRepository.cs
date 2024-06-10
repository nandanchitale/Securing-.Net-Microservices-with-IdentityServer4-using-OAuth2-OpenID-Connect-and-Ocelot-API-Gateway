using Movies.API.Data.IRepository;
using Movies.API.Model;
using Movies.API.Utils.Constants;

namespace Movies.API.Data.Repository;

public class MoviesRepository : IMoviesRepository
{
    private readonly AppDbContext _context;
    public MoviesRepository(AppDbContext context)
    {
        _context = context;
    }
    public void CreateMovie(Movie movie)
    {
        if (movie == null) throw new ArgumentNullException(nameof(movie));
        _context.Add(movie);
    }

    public IEnumerable<Movie> GetAllMatchingMovies(string movieName)
    {
        if (movieName == null) throw new ArgumentNullException(nameof(movieName));

        return _context.Movies.Where(
            rec => rec.Title.ToLower().Equals(movieName.ToLower())
            && rec.Status.Equals(Status.ACTIVE)
        ).AsEnumerable();
    }

    public IEnumerable<Movie> GetAllMovies()
    {
        return _context.Movies.Where(rec=>rec.Status.Equals(Status.ACTIVE)).AsEnumerable().OrderBy(rec => rec.Id);
    }

    public Movie? GetMovieById(int id)
    {
        return _context.Movies.Where(
            rec => rec.Id.Equals(id) 
            && rec.Status.Equals(Status.ACTIVE)
        ).FirstOrDefault();
    }

    public (bool, Movie) isMovieAlreadyExists(string movieName){
        bool isMovieActive = false;
        Movie movie = _context.Movies.Where(
            rec=>rec.Title.ToLower().Equals(movieName.ToLower())
        ).FirstOrDefault();

        if(movie is not null) isMovieActive = movie.Status.Equals(Status.ACTIVE);

        return (isMovieActive, movie);
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}