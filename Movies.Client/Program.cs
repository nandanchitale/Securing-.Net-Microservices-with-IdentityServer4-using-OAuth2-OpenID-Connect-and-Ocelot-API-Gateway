using System.Net;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Movies.Client.ApiServices.Implementation;
using Movies.Client.ApiServices.Interfaces;
using Movies.Client.HttpHandlers;
using Movies.DataAccess.IRepository;
using Movies.DataAccess.Repository;

var builder = WebApplication.CreateBuilder(args);

string IdentityServerUrl = builder.Configuration["IdentityServer:Host"];
string DatabaseConnectionString = builder.Configuration.GetConnectionString("MoviesAPIConnection");

Console.WriteLine($"--> IdentityServerUrl : {IdentityServerUrl}");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Automapper DI
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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

        // options.Scope.Add("openid");
        // options.Scope.Add("profile");
        options.Scope.Add("address");
        options.Scope.Add("email");

        options.Scope.Add(builder.Configuration["IdentityServer:Scope"]);

        options.Scope.Add("roles");

        options.ClaimActions.MapUniqueJsonKey("role", "role");

        options.GetClaimsFromUserInfoEndpoint = true;

        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = JwtClaimTypes.GivenName,
            RoleClaimType = JwtClaimTypes.Role
        };
    });

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Repository DI
builder.Services.AddScoped<IMovieService, MovieApiService>();
Console.WriteLine("--> Loaded DI For MoviesRepository");

// Services
builder.Services.AddScoped<IMovieService, MovieApiService>();
Console.WriteLine("--> Loaded DI For MovieService");

// Create HttpClient used for accessing movies api
builder.Services.AddTransient<AuthenticationDelegationHandler>();
Console.WriteLine("Creating AuthenticationDelegationHandler for Accessing Movies API");

Console.WriteLine($"Creating HttpClient to communicate with MoviesAPI : {builder.Configuration["IdentityServer:ClientId"]}");
builder.Services
    .AddHttpClient(builder.Configuration["IdentityServer:ClientId"], client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["MoviesAPI"]);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    })
    .AddHttpMessageHandler<AuthenticationDelegationHandler>();

Console.WriteLine($"Created HttpClient to communicate with MoviesAPI : {builder.Configuration["IdentityServer:ClientId"]}");

// HttpClient to access Identity Server
builder.Services
    .AddHttpClient(builder.Configuration["IdentityServer:Id"], client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["IdentityServer:Host"]);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    });
Console.WriteLine($"Created HttpClient to communicate with Identity Server : {builder.Configuration["IdentityServer:Id"]}");

builder.Services.AddHttpContextAccessor();

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

// Redirec user if Unauthorized
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == (int)HttpStatusCode.Unauthorized ||
            response.StatusCode == (int)HttpStatusCode.Forbidden)
        response.Redirect("/UnAuthorized");
});

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
