using Microsoft.EntityFrameworkCore;
using Movie = Movies.Models.Movies;

namespace Movies.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Movie> Movies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Set the default schema for all tables
        modelBuilder.HasDefaultSchema("movies");

        modelBuilder.Entity<Movie>()
        .Property(m => m.ReleaseDate)
        .HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
    }
}