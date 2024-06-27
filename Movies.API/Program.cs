using System.IdentityModel.Tokens.Jwt;
using System.Text;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Movies.DataAccess;
using Movies.DataAccess.IRepository;
using Movies.DataAccess.Repository;

var builder = WebApplication.CreateBuilder(args);

string IdentityServerUrl = builder.Configuration.GetSection("IdentityServer:Host").Value;
string DatabaseConnectionString = builder.Configuration.GetConnectionString("MoviesAPIConnection");

Console.WriteLine($"--> IdentityServerUrl : {IdentityServerUrl}");

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

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
        // options.Audience = "moviesAPI";
        options.RequireHttpsMetadata = false;
        options.UseSecurityTokenValidators = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidIssuer = IdentityServerUrl,

            ValidateAudience = false,
            ValidAudience = "moviesAPI",

            ValidateIssuerSigningKey = false,

            //IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey)),
            //comment this and add this line to fool the validation logic
            SignatureValidator = delegate (string token, TokenValidationParameters parameters)
            {
                var jwt = new JsonWebToken(token);

                return jwt;
            },

            RequireExpirationTime = true,
            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero,
        };
    });


// // builder.Services.AddAuthorization(options =>
// //     {
// //         options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "moviesAPI", builder.Configuration.GetSection("IdentityServer:ClientId").Value));
// //     });

// Add Claim based Authorization
builder.Services
    .AddAuthorization(
        options => options.AddPolicy(
            "ClientIdPolicy",
            policy => policy.RequireClaim("client_id", "moviesClient", "movies_mvc_client")
        )
    );

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
// app.ValidateJWTToken();
app.UseAuthentication(); // this one first
app.UseAuthorization();
app.MapControllers().RequireAuthorization("ClientIdPolicy");

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();
