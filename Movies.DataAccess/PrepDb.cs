using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Movie = Movies.Models.Movies;
using Microsoft.AspNetCore.Builder;

namespace Movies.DataAccess;

public static class PrepDb
{

    ///
    public static void PrepPopulation(
        IApplicationBuilder app,
        bool isProd
    )
    {
        using (
            IServiceScope serviceScope = app.ApplicationServices.CreateScope()
        )
        {
            SeedData(
                serviceScope.ServiceProvider.GetService<AppDbContext>(),
                isProd
            );
        }
    }

    private static void SeedData(
        AppDbContext context,
        bool isProd
    )
    {
        try
        {
            if (isProd)
            {
                Console.WriteLine("--> Attempting to apply migrations to Postgresql Database Server...");
                context.Database.Migrate();
                Console.WriteLine("--> Migration Applied to Postgresql Database Server...");
            }
            if (!context.Movies.Any())
            {
                Console.WriteLine("--> Seeding Data...");
                context.Movies.AddRange(
                    new Movie()
                    {
                        Title = "The Shawshank Redemption",
                        Genre = "Drama",
                        ReleaseDate = DateTime.Parse("1994-09-23"),
                        Owner = "John Doe",
                    },
                    new Movie()
                    {
                        Title = "The Godfather",
                        Genre = "Crime",
                        ReleaseDate = DateTime.Parse("1972-03-15"),
                        Owner = "Jane Smith",
                    },

                    new Movie()
                    {
                        Title = "Inception",
                        Genre = "Action",
                        ReleaseDate = DateTime.Parse("2010-07-16"),
                        Owner = "Bob Johnson",
                    }
                );
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.WriteLine($"Exception at PrepDb > SeedData() => {ex.Message}");
        }
    }
}