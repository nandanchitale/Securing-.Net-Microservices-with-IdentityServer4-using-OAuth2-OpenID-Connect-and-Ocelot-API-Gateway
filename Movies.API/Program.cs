using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Movies.API.Data;
using Movies.API.Data.IRepository;
using Movies.API.Data.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add Controllers Serivce
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Movies Microservice API",
        Description = "An ASP.NET Core Web API for Movies Microservices",
    });

    options.ResolveConflictingActions(apiDescription => apiDescription.First());
});

// Setup Postgresql database
if (builder.Environment.IsProduction())
{
    Console.WriteLine($"--> Application Environment IsProduction ? {builder.Environment.IsProduction()}");
    Console.WriteLine("--> Using Postgresql DB");
    Console.WriteLine($"--> Connection String : {builder.Configuration.GetConnectionString("MoviesAPIConnection")}");

    _ = builder.Services.AddDbContext<AppDbContext>(
        opt => opt.UseNpgsql(
            builder.Configuration.GetConnectionString("MoviesAPIConnection")
        )
    );
}
// Setup in memory database
else
{
    Console.WriteLine("--> Using InMem DB");
    _ = builder.Services.AddDbContext<AppDbContext>(
        opt => opt.UseInMemoryDatabase("InMemoryDb")
    );
}

// Repository DI
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();

// Automapper DI
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = String.Empty;
    });

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();


