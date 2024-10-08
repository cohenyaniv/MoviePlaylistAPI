using Microsoft.Azure.Cosmos;
using MoviePlaylist.Repositories;
using MoviePlaylist.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(); // Registers controllers

// 1. Register CosmosDB Client
builder.Services.AddSingleton(s =>
{
    var configuration = s.GetRequiredService<IConfiguration>();
    string account = configuration["CosmosDb:Account"];
    string key = configuration["CosmosDb:Key"];
    CosmosClient cosmosClient = new CosmosClient(account, key);
    return cosmosClient;
});

// 2. Register Repositories
builder.Services.AddSingleton<IPlaylistRepository, PlaylistRepository>(s =>
{
    var cosmosClient = s.GetRequiredService<CosmosClient>();
    string databaseName = builder.Configuration["CosmosDb:DatabaseName"];
    string containerName = builder.Configuration["CosmosDb:ContainerName"];
    return new PlaylistRepository(cosmosClient, databaseName, containerName);
});

builder.Services.AddSingleton<ITrackRepository, TrackRepository>(s =>
{
    var cosmosClient = s.GetRequiredService<CosmosClient>();
    string databaseName = builder.Configuration["CosmosDb:DatabaseName"];
    string containerName = builder.Configuration["CosmosDb:TrackContainerName"];  // Assuming you have a separate container for tracks.
    return new TrackRepository(cosmosClient, databaseName, containerName);
});

// 3. Register Services
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<ITrackService, TrackService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast =  Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast")
//.WithOpenApi();
// Map controller endpoints

app.MapControllers(); // This tells the app to use the controllers
app.Run();

//record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}
