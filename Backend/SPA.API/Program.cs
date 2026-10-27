
using Comment.API.EndPoints;
using SPA.API.Endpoints;
using SPA.API.Extensions;
using User.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Exception Handlers
builder.Services.AddExeptionHandlers();

// Services
builder.Services.AddSignalRElement();
builder.Services.AddApiVersioningConfiguration();

// Modules
builder.Services.AddModules(builder.Configuration);

// Authefication/Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();

//Limiter
builder.Services.AddRateLimiting();

//ReactRoute
builder.Services.AddReactCors();

var app = builder.Build();

// Global Exception Handler
app.UseExceptionHandler();

// Seeders
await app.SeedModulesAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseReactCors();

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

// Map endpoints
app.MapCommentRealtime();
app.MapCommentEndpoints();
app.MapUserEndpoints();
app.MapCaptchaEndpoints();

app.Run();
