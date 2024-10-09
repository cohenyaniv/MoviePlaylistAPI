using Microsoft.Azure.Cosmos;
using MoviePlaylist.DBContexts;
using MoviePlaylist.Repositories;
using MoviePlaylist.Services;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(); // Registers controllers

// Blob Storage configuration (Replace with actual values)
var blobConnectionString = builder.Configuration["AzureBlobStorage:ConnectionString"];
var blobContainerName = builder.Configuration["AzureBlobStorage:ContainerName"];

// Queue Storage configuration (Replace with actual values)
var queueConnectionString = builder.Configuration["AzureQueueStorage:ConnectionString"];
var queueName = builder.Configuration["AzureQueueStorage:QueueName"];

// Register the Blob Storage service
builder.Services.AddSingleton(s => new BlobStorageService(blobConnectionString, blobContainerName));

// Register the Queue service
builder.Services.AddSingleton(s => new QueueService(queueConnectionString, queueName));

// Register the background service for processing the queue (singleton by default)
builder.Services.AddSingleton<QueueProcessorService>();

// Add hosted service (QueueProcessorService acts as the background task)
builder.Services.AddHostedService(provider =>
    provider.GetRequiredService<QueueProcessorService>());

// 1. Register CosmosDB Client
builder.Services.AddSingleton(s =>
{
    var configuration = s.GetRequiredService<IConfiguration>();
    string account = configuration["UserActionDB:Account"];
    string key = configuration["UserActionDB:Key"];
    CosmosClient cosmosClient = new CosmosClient(account, key);
    return cosmosClient;
});

builder.Services.AddSingleton<IBlobStorageService>(s =>
    new BlobStorageService(blobConnectionString, blobContainerName));

// 2. Register Repositories
// Register Repositories
builder.Services.AddScoped<IUserHistoryRepository>(s =>
{
    var configuration = s.GetRequiredService<IConfiguration>();
    string blobConnectionString = configuration["AzureBlobStorage:ConnectionString"];
    string containerId = configuration["AzureBlobStorage:ContainerName"];
    // Create BlobServiceClient
    var blobServiceClient = new BlobServiceClient(blobConnectionString);
    
    var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerId);

    return new UserHistoryRepository(blobContainerClient);
});

builder.Services.AddScoped<IUserPlaylistRepository>(s =>
{
    var configuration = s.GetRequiredService<IConfiguration>();
    var cosmosClient = s.GetRequiredService<CosmosClient>();
    string databaseId = configuration["UserActionDB:DatabaseName"];
    string containerId = configuration["UserActionDB:ContainerName"];
    return new UserPlaylistRepository(cosmosClient, databaseId, containerId);
});

builder.Services.AddScoped<IPlaylistRepository>(s =>
{
    var configuration = s.GetRequiredService<IConfiguration>();  
    var cosmosClient = s.GetRequiredService<CosmosClient>();  
    string databaseName = configuration["PlayListDB:DatabaseName"];  
    string containerName = configuration["PlayListDB:ContainerName"];  

    // Pass all dependencies to the PlaylistRepository constructor
    return new PlaylistRepository(cosmosClient, databaseName, containerName);
});

builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<ITrackService, TrackService>();

// 3. Register Services
builder.Services.AddScoped<IPlaylistService, PlaylistService>();



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
