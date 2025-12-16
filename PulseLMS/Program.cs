using System.Text;
using System.Net;
using System.Net.Sockets;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PulseLMS.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PulseLMS.Common;
using PulseLMS.Features.Categories.Services;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var projectRef = builder.Configuration["Supabase:ProjectRef"]!;
var issuer = $"https://{projectRef}.supabase.co/auth/v1";

// Configure logging early so we can see detailed startup info
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(builder.Environment.IsDevelopment() ? LogLevel.Debug : LogLevel.Information);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Supabase publishes OpenID metadata here:
        options.Authority = issuer;
        options.MetadataAddress = $"{issuer}/.well-known/openid-configuration";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,

            // Supabase tokens usually use aud = "authenticated"
            ValidateAudience = true,
            ValidAudience = "authenticated",

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            NameClaimType = "sub",
            ClockSkew = TimeSpan.FromMinutes(2),
            RoleClaimType = "user_role"
        };
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("AdminOnly", p => p.RequireClaim("user_role", "admin"));
    o.AddPolicy("AuthorOrAdmin", p => p.RequireClaim("user_role", "admin", "author"));
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<AuditSaveChangesInterceptor>();
builder.Services.AddScoped<CategoryService>();

// Register fluent validation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var cs = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>((sp, opt) =>
{
    opt.UseNpgsql(cs);
    if (builder.Environment.IsDevelopment())
    {
        opt.EnableDetailedErrors();
        opt.EnableSensitiveDataLogging();
    }
    opt.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
});

var app = builder.Build();

// Log effective environment and connection details
string MaskConnectionString(string connectionString)
{
    try
    {
        var b = new NpgsqlConnectionStringBuilder(connectionString);
        if (!string.IsNullOrEmpty(b.Password)) b.Password = "***";
        return b.ToString();
    }
    catch
    {
        return connectionString;
    }
}

app.Logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
app.Logger.LogInformation("Supabase ProjectRef: {ProjectRef}", projectRef);
app.Logger.LogInformation("Using connection string (Default): {ConnectionString}", MaskConnectionString(cs ?? ""));
app.Logger.LogInformation("OSSupportsIPv6={IPv6}, OSSupportsIPv4={IPv4}", Socket.OSSupportsIPv6, Socket.OSSupportsIPv4);

try
{
    // Resolve DB host addresses to see what the client will try
    var parsed = new NpgsqlConnectionStringBuilder(cs ?? "");
    if (!string.IsNullOrWhiteSpace(parsed.Host))
    {
        var addresses = Dns.GetHostAddresses(parsed.Host);
        foreach (var addr in addresses)
        {
            app.Logger.LogInformation("DNS resolved {Host} -> {Address}", parsed.Host, addr);
        }
    }
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "Failed to resolve database host addresses.");
}

// Log the actual DbConnection string constructed by EF/Npgsql (masked)
try
{
    using var scope = app.Services.CreateScope();
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var actual = ctx.Database.GetDbConnection().ConnectionString;
    app.Logger.LogInformation("EF DbConnection.ConnectionString: {ConnectionString}", MaskConnectionString(actual));
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "Unable to obtain EF DbConnection at startup.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapControllers();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
   {
       options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
       options.RoutePrefix = string.Empty;
   });
}

app.Run();

