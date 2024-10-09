using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using MoviePlaylist.Repositories;
using MoviePlaylist.DBContexts;
using Microsoft.Extensions.DependencyInjection;

namespace MoviePlaylist.Services
{
    public class QueueProcessorService : BackgroundService
    {
        private readonly QueueService _queueService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IBlobStorageService _blobStorageService;

        // Constructor to inject necessary services
        public QueueProcessorService(QueueService queueService, IServiceScopeFactory playlistRepositoryFactory, IBlobStorageService blobStorageService)
        {
            _queueService = queueService;
            _serviceScopeFactory = playlistRepositoryFactory;
            _blobStorageService = blobStorageService;
        }

        // This method continuously listens to the queue and processes messages
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Get the next playlist ID from the queue
                var playlistId = await _queueService.ReceiveMessageAsync();

                if (!string.IsNullOrEmpty(playlistId))
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var playlistRepository = scope.ServiceProvider.GetRequiredService<IPlaylistRepository>();
                        var playlist = await playlistRepository.GetPlaylistByIdAsync(playlistId);

                        // Retrieve the playlist by ID from CosmosDB

                        if (playlist != null)
                        {
                            // Archive the playlist to Blob Storage
                            await _blobStorageService.ArchivePlaylistAsync(playlist);
                        }
                    }
                }

                // Delay between queue polling (5 seconds)
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
