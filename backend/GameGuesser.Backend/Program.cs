using GameGuesser.Backend.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(new JsonSerializerOptions()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});
builder.Services.AddSingleton<ConfigManager>();
builder.Services.AddSingleton<Random>();
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy("debug", p =>
    {
        p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod();
    });
});
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("debug");
    app.UseFileServer();
}
else
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();