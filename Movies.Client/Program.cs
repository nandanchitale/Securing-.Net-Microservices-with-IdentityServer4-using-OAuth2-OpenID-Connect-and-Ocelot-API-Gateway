using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movies.Client.ApiServices.Implementation;
using Movies.Client.ApiServices.Interfaces;
using Movies.DataAccess;
using Movies.DataAccess.IRepository;
using Movies.DataAccess.Repository;

var builder = WebApplication.CreateBuilder(args);

string IdentityServerUrl = builder.Configuration.GetSection("IdentityServerURL").Value;
string DatabaseConnectionString = builder.Configuration.GetConnectionString("MoviesAPIConnection");

Console.WriteLine($"--> IdentityServerUrl : {IdentityServerUrl}");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Authentication with OpenIdConnect
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Authority = IdentityServerUrl;
        options.RequireHttpsMetadata = false;
        options.ClientId = "movies_mvc_client";
        options.ClientSecret = "secret";
        options.ResponseType = "code";

        options.Scope.Add("openid");
        options.Scope.Add("profile");

        options.GetClaimsFromUserInfoEndpoint = true;
    });

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Setup Postgresql database
if (builder.Environment.IsProduction())
{
    Console.WriteLine(
        $"--> Application Environment IsProduction ? {builder.Environment.IsProduction()}"
    );
    Console.WriteLine("--> Using Postgresql DB");
    Console.WriteLine($"--> Connection String : {DatabaseConnectionString}");

    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseNpgsql(DatabaseConnectionString, b => b.MigrationsAssembly("Movies.API"))
    );
}
else
{
    Console.WriteLine("--> Using InMem DB");
    _ = builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMemoryDb"));
}

// Repository DI
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();

// Services
builder.Services.AddScoped<IMovieService, MovieApiService>();

// Automapper DI
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
