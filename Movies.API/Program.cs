using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Movies.DataAccess;
using Movies.DataAccess.IRepository;
using Movies.DataAccess.Repository;

var builder = WebApplication.CreateBuilder(args);

string IdentityServerUrl = builder.Configuration.GetSection("IdentityServerURL").Value;
string DatabaseConnectionString = builder.Configuration.GetConnectionString("MoviesAPIConnection");

Console.WriteLine($"--> IdentityServerUrl : {IdentityServerUrl}");

// Add services to the container.
builder.Services.AddControllers();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = IdentityServerUrl;
        options.Audience = "moviesAPI";
        options.RequireHttpsMetadata = false;
        options.UseSecurityTokenValidators = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            // ValidIssuer = IdentityServerUrl,
            // ValidateAudience = true,
            // ValidAudience = ProjectProperties.Audience
            LogValidationExceptions = false,
            ValidateIssuerSigningKey = false,
            ValidateAudience = false
        };
    });

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
    Console.WriteLine($"--> Connection String : {DatabaseConnectionString}");

    builder.Services.AddDbContext<AppDbContext>(
        opt => opt.UseNpgsql(DatabaseConnectionString, b => b.MigrationsAssembly("Movies.API"))
    );
}
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
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = String.Empty;
    });

    app.UseSwagger();
    app.UseSwaggerUI();
}


// app.UseRouting();

app.UseRouting();
app.UseAuthentication(); // this one first
app.UseAuthorization();
app.MapControllers();

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();
