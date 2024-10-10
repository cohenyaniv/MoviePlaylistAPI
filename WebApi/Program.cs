using Microsoft.Azure.Cosmos;
using MoviePlaylist.Repositories;
using MoviePlaylist.Services;
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(); // Registers controllers

// Blob Storage configuration (Replace with actual values)
var blobConnectionString = builder.Configuration["AzureBlobStorage:ConnectionString"];
var blobContainerName = builder.Configuration["AzureBlobStorage:ContainerName"];

// Queue Storage configuration (Replace with actual values)
var queueConnectionString = builder.Configuration["AzureQueueStorage:ConnectionString"];
var queueName = builder.Configuration["AzureQueueStorage:QueueName"];

// Register the Queue service
builder.Services.AddSingleton<IQueueService>(s => new QueueService(queueConnectionString, queueName));

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
    string databaseId = configuration["PlayListDB:DatabaseName"];
    string containerId = configuration["PlayListDB:ContainerName"];
    return new UserPlaylistRepository(cosmosClient, databaseId, containerId);
});

// The counter service
builder.Services.AddSingleton<UserCounterService>();
builder.Services.AddHostedService<UserCounterService>();

builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();

builder.Services.AddScoped<IPlaylistService, PlaylistService>();
//builder.Services.AddScoped<ITrackService, TrackService>();

// 3. Register Services
builder.Services.AddScoped<IPlaylistService, PlaylistService>();

var app = builder.Build();

// Register the middleware before UseSwagger or other middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers(); // This tells the app to use the controllers
app.Run();
