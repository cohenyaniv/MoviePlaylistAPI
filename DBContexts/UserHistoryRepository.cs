using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Models;

namespace MoviePlaylist.Repositories
{
    /// <summary>
    /// Repository implementation for managing user-specific playlist progress in CosmosDB.
    /// </summary>
    public class UserHistoryRepository : IUserHistoryRepository
    {
        private readonly BlobContainerClient _blobClient;

        /// <summary>
        /// Initializes a new instance of the UserPlaylistRepository with the given CosmosDB container.
        /// </summary>
        /// <param name="cosmosClient">The CosmosClient instance for interacting with CosmosDB.</param>
        /// <param name="databaseId">The ID of the CosmosDB database.</param>
        /// <param name="containerId">The ID of the CosmosDB container.</param>
        public UserHistoryRepository(BlobContainerClient blobClient)
        {
            _blobClient = blobClient;
        }

        /// <summary>
        /// Saves or updates the user-specific playlist progress.
        /// </summary>
        /// <param name="userPlaylist">The user's playlist progress object to save.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public async Task SaveUserPlaylistAsync(UserCurrentPlaylist userPlaylist)
        {
            // Create an AppendBlobClient for the specified blob name
            AppendBlobClient appendBlobClient = _blobClient.GetAppendBlobClient(userPlaylist.UserId);

            // Check if the blob exists; if not, create it
            if (!await appendBlobClient.ExistsAsync())
            {
                await appendBlobClient.CreateAsync();
            }

            // Convert the string content to a byte array
            byte[] byteArray = Encoding.UTF8.GetBytes($"ActionTime {DateTime.UtcNow}, User {userPlaylist.UserId} {userPlaylist.Status} at track {userPlaylist.CurrentTrackIndex} segment {userPlaylist.CurrentSegmentIndex}"  + Environment.NewLine); // Add newline for separation
            using (var stream = new MemoryStream(byteArray))
            {
                // Append the text to the blob
                await appendBlobClient.AppendBlockAsync(stream);
            }
        }

        /// <summary>
        /// Retrieves the interaction history for a user in a specific playlist.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <returns>A task representing the asynchronous operation, containing a list of interaction history records.</returns>
        public async Task<string> GetUserHistoryAsync(string userId)
        {
            BlobClient blobclient = _blobClient.GetBlobClient(userId);
            // Download the blob's content as a stream
            BlobDownloadInfo blobDownload = await blobclient.DownloadAsync();

            using (StreamReader reader = new StreamReader(blobDownload.Content))
            {
                string content = await reader.ReadToEndAsync();
                return content;
            }
        }
    }
}
