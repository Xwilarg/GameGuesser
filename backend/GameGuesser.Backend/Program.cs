using GameGuesser.Backend.Database.Context;
using GameGuesser.Backend.Database.Queries;
using GameGuesser.Backend.Database.Works;
using GameGuesser.Backend.Interfaces;
using GameGuesser.Backend.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(new JsonSerializerOptions()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});
builder.Services.AddSingleton<ConfigManager>();
builder.Services.AddSingleton<IHttpHandler, GameGuesser.Backend.Services.HttpClientHandler>();
builder.Services.AddDbContext<SqliteContext>();

// Works
builder.Services
    .AddScoped<InitWork>()
    .AddScoped<ConfigWork>()
    .AddScoped<LocalConfigWork>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("debug", p =>
    {
        p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod();
    });
});
builder.Services.AddControllers();

var app = builder.Build();

using var scope = app.Services.CreateScope();
await scope.ServiceProvider.GetRequiredService<InitWork>().InitAsync();

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