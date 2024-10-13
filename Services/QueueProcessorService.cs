using Microsoft.Extensions.Hosting;
using MoviePlaylist.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace MoviePlaylist.Services
{
    public class QueueProcessorService : BackgroundService
    {
        private readonly IQueueService _queueService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceScopeFactory _historyBlobFactory;

        // Constructor to inject necessary services
        public QueueProcessorService(IQueueService queueService, IServiceScopeFactory playlistRepositoryFactory, IServiceScopeFactory historyBlobFactory)
        {
            _queueService = queueService;
            _serviceScopeFactory = playlistRepositoryFactory;
            _historyBlobFactory = historyBlobFactory;
        }

        // This method continuously listens to the queue and processes messages
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Get the next playlist ID from the queue
                var userCurrentPlayList = await _queueService.ReceiveMessageAsync();

                if (userCurrentPlayList != null)
                {
                    // Update the user status
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var playlistRepository = scope.ServiceProvider.GetRequiredService<IUserPlaylistRepository>();
                        switch (userCurrentPlayList.Status)
                        {
                            case Models.PlaylistStatus.Attached:
                                await playlistRepository.AddPlaylistAsync(userCurrentPlayList);
                                break;
                            default:
                                await playlistRepository.SaveUserPlaylistAsync(userCurrentPlayList);
                                break;
                        }

                        //await playlistRepository.SaveUserPlaylistAsync(userCurrentPlayList);
                    }

                    // Write to history
                    using (var scope = _historyBlobFactory.CreateScope())
                    {
                        var historyRepository = scope.ServiceProvider.GetRequiredService<IUserHistoryRepository>();
                        // Archive the playlist to Blob Storage
                        await historyRepository.SaveUserPlaylistAsync(userCurrentPlayList);
                    }
                }

                // Delay between queue polling (5 seconds)
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
