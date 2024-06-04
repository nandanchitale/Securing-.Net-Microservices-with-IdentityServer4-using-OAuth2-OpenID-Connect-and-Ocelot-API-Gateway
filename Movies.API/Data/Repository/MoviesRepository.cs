using Movies.API.Data.IRepository;
using Movies.API.Model;

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
        ).AsEnumerable();
    }

    public IEnumerable<Movie> GetAllMovies()
    {
        return _context.Movies.AsEnumerable().OrderBy(rec => rec.Id);
    }

    public Movie? GetMovieById(int id)
    {
        return _context.Movies.Where(
            rec => rec.Id.Equals(id)
        ).FirstOrDefault();
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}