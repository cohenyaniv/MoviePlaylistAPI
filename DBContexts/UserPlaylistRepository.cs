using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Models;
using MoviePlaylist.Models;

namespace MoviePlaylist.Repositories
{
    /// <summary>
    /// Repository implementation for managing user-specific playlist progress in CosmosDB.
    /// </summary>
    public class UserPlaylistRepository : IUserPlaylistRepository
    {
        private readonly BlobContainerClient _blobClient;

        /// <summary>
        /// Initializes a new instance of the UserPlaylistRepository with the given CosmosDB container.
        /// </summary>
        /// <param name="cosmosClient">The CosmosClient instance for interacting with CosmosDB.</param>
        /// <param name="databaseId">The ID of the CosmosDB database.</param>
        /// <param name="containerId">The ID of the CosmosDB container.</param>
        public UserPlaylistRepository(BlobContainerClient blobClient)
        {
            _blobClient = blobClient;
        }

        /// <summary>
        /// Retrieves the user-specific playlist progress by user ID and playlist ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <returns>A task representing the asynchronous operation, containing the user's playlist progress.</returns>
        public async Task<UserCurrentPlaylist> GetUserPlaylistAsync(string playlistId, string userId)
        {
            try
            {
                //var response = await _container.ReadItemAsync<UserCurrentPlaylist>(playlistId, PartitionKey.None);
                //return response.Resource;
                return null;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // The playlist is not attached to user
                return null; // Handle not found case
            }
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
                //await _container.UpsertItemAsync(userPlaylist, PartitionKey.None);
            }
            catch (Exception)
            {
                throw;
            }            
        }

        /// <summary>
        /// Retrieves the interaction history for a user in a specific playlist.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <returns>A task representing the asynchronous operation, containing a list of interaction history records.</returns>
        public async Task<List<InteractionHistory>> GetInteractionHistoryAsync(string userId, string playlistId)
        {
            //var query = new QueryDefinition("SELECT * FROM c WHERE c.playlistId = @playlistId AND c.userId = @userId")
            //    .WithParameter("@playlistId", playlistId)
            //    .WithParameter("@userId", userId);

            //var iterator = _container.GetItemQueryIterator<InteractionHistory>(query);

            var results = new List<InteractionHistory>();
            //while (iterator.HasMoreResults)
            //{
            //    var response = await iterator.ReadNextAsync();
            //    results.AddRange(response.Resource);
            //}

            return results;
        }
    }
}
