using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Movies.API.Data;
using Movies.API.Data.IRepository;
using Movies.API.Data.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add Controllers Serivce
builder.Services.AddControllers();

// Add JWT Authentication
builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration.GetSection("IdentityServerURL").Value;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
        options.RequireHttpsMetadata = false;
    });

// Add Claim based Authorization
builder.Services
    .AddAuthorization(
        options => options.AddPolicy(
            "ClientIdPolicy",
            policy => policy.RequireClaim("client_id", "moviesClient")
        )
    );
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle builder.Services.AddEndpointsApiExplorer();
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
if (builder.Environment.IsProduction()) // Comment this line and else block while applying migrations 
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

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app, app.Environment.IsProduction()); // Comment this line while applying migrations

app.Run();