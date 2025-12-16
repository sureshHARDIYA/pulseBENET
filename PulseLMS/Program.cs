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

// Disable console logging
builder.Logging.ClearProviders();

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
         options.RoutePrefix = "swagger";
   });
}
 
 // Root endpoint
 app.MapGet("/", () => Results.Json(new
 {
     Message = "Silence is golden!",
     swagger = "https://sureshhardiya.github.io/pulseBENET/"
 }));

app.Run();

