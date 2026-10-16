
using Comment.API.EndPoints;
using SPA.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddApiVersioningConfiguration();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map endpoints
app.MapCommentEndpoints();

app.Run();
