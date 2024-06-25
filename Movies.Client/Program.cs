using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Movies.Client.ApiServices.Implementation;
using Movies.Client.ApiServices.Interfaces;
using Movies.Client.HttpHandlers;
using Movies.DataAccess;
using Movies.DataAccess.IRepository;
using Movies.DataAccess.Repository;

var builder = WebApplication.CreateBuilder(args);

string IdentityServerUrl = builder.Configuration["IdentityServer:Host"];
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
        options.ClientId = builder.Configuration["IdentityServer:ClientId"];
        options.ClientSecret = builder.Configuration["IdentityServer:ClientSecret"];
        options.ResponseType = "code id_token";

        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add(builder.Configuration["IdentityServer:Scope"]);

        options.GetClaimsFromUserInfoEndpoint = true;
    });

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

Console.WriteLine("--> Using InMem DB");
_ = builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMemoryDb"));

// Repository DI
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();

// Services
builder.Services.AddScoped<IMovieService, MovieApiService>();
builder.Services.AddScoped<IApiHandlerService, ApiHandlerService>();

// Automapper DI
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Create HttpClient used for accessing movies api
builder.Services.AddTransient<AuthenticationDelegationHandler>();

builder.Services
    .AddHttpClient(builder.Configuration["IdentityServer:ClientId"], client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["MoviesAPI"]);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    })
    .AddHttpMessageHandler<AuthenticationDelegationHandler>();

// HttpClient to access Identity Server
builder.Services
    .AddHttpClient(builder.Configuration["IdentityServer:Id"], client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["IdentityServer:Host"]);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    });

builder.Services.AddHttpContextAccessor();

// builder.Services.AddSingleton(new ClientCredentialsTokenRequest
// {
//     Address = $"{builder.Configuration["IdentityServer:Host"]}/connect/token",
//     ClientId = builder.Configuration["IdentityServer:ClientId"],
//     ClientSecret = builder.Configuration["IdentityServer:ClientSecret"],
//     Scope = builder.Configuration["IdentityServer:Scope"],
// });

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
