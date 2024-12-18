using System.Collections.Immutable;

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// app.MapGet("/bands", () => [band]).WithName("GetBands");

var songs = GenerateSongs();
var setLists = GenerateSetLists(songs);
var band = new Band(Guid.NewGuid(), "A Cool Band Name", songs, setLists);
app.MapGet("/songs", ([FromQuery(Name = "bandId")] Guid bandId) => {
    return songs;
})
.WithName("GetSongsForBand");

app.MapGet("/setLists", ([FromQuery(Name = "bandId")] Guid bandId) => {
    return setLists;
})
.WithName("GetSetListsFromBand");

app.MapDefaultEndpoints();

app.Run();

static ImmutableArray<Song> GenerateSongs(int amount = 20)
{
    var songs = new Song[amount];
    for (var i = 0; i < amount; i++)
    {
        songs[i] = new Song(
            Guid.NewGuid(), 
            $"Song {i}", 
            TimeSpan.FromSeconds(Random.Shared.Next(700)));
    }

    return songs.ToImmutableArray();
}

static ImmutableArray<SetList> GenerateSetLists(ImmutableArray<Song> songs, int amount = 5)
{
    var setLists = new SetList[amount];
    for (var i = 0; i < amount; i++)
    {
        var a = Random.Shared.Next(songs.Length);
        var b = Random.Shared.Next(songs.Length);
        var start = Math.Min(a, b);
        var end = Math.Max(a, b);

        setLists[i] = new SetList(
            Guid.NewGuid(),
            $"Set List {i}",
            songs[start..end].ToImmutableArray()
        );
    }

    return setLists.ToImmutableArray();
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public record Song(Guid Id, string Name, TimeSpan Duration)
{
}

public record SetList(Guid Id, string Name, ImmutableArray<Song> Songs)
{
}

public record Band(Guid Id, string Name, ImmutableArray<Song> Songs, ImmutableArray<SetList> SetLists)
{
}