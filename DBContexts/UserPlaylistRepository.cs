using Microsoft.Azure.Cosmos;
using Models;

namespace MoviePlaylist.Repositories
{
    /// <summary>
    /// Repository implementation for managing user-specific playlist progress in CosmosDB.
    /// </summary>
    public class UserPlaylistRepository : IUserPlaylistRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        /// <summary>
        /// Initializes a new instance of the UserPlaylistRepository with the given CosmosDB container.
        /// </summary>
        /// <param name="cosmosClient">The CosmosClient instance for interacting with CosmosDB.</param>
        /// <param name="databaseId">The ID of the CosmosDB database.</param>
        /// <param name="containerId">The ID of the CosmosDB container.</param>
        public UserPlaylistRepository(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(databaseName, containerName);
        }

        public async Task AddPlaylistAsync(UserCurrentPlaylist playlist)
        {
            playlist.LastStoppedAt = DateTime.UtcNow;
            await _container.UpsertItemAsync(playlist, PartitionKey.None);
        }


        /// <summary>
        /// Saves or updates the user-specific playlist progress.
        /// </summary>
        /// <param name="userPlaylist">The user's playlist progress object to save.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public async Task SaveUserPlaylistAsync(UserCurrentPlaylist userPlaylist)
        {
            try
            {
                await _container.UpsertItemAsync(userPlaylist, PartitionKey.None);
            }
            catch (CosmosException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves the user-specific playlist progress by user ID and playlist ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <returns>A task representing the asynchronous operation, containing the user's playlist progress.</returns>
        public async Task<UserCurrentPlaylist> GetUserPlaylistAsync(string userId)
        {
            try
            {
                var response = await _container.ReadItemAsync<UserCurrentPlaylist>(userId,PartitionKey.None);
                return response.Resource;
                //return null;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // The playlist is not attached to user
                return null; // Handle not found case
            }
        }
    }
}
