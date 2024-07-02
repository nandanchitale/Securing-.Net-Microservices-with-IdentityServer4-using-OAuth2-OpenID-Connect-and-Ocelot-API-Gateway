using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Load the ocelot.json configuration file
builder.Configuration.AddJsonFile("ocelot.Developement.json", optional: false, reloadOnChange: true);

string IdentityServerUrl = builder.Configuration.GetSection("IdentityServer:Host").Value;

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

string AuthenticationProviderKey = "IdentityApiKey";

builder.Services
    .AddAuthentication()
    .AddJwtBearer(AuthenticationProviderKey, options =>
    {
        options.Authority = IdentityServerUrl;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

// Add Ocelot services
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Use Ocelot
await app.UseOcelot(); // Updated

app.Run();
