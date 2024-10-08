using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using MoviePlaylist.Models;

namespace MoviePlaylist.Repositories
{
    /// <summary>
    /// Implementation of the IPlaylistRepository interface using CosmosDB.
    /// </summary>
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly Microsoft.Azure.Cosmos.Container _container;

        public PlaylistRepository(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            _container = cosmosClient.GetContainer(databaseName, containerName);
        }

        public async Task<Playlist> AddPlaylistAsync(Playlist playlist)
        {
            var response = await _container.CreateItemAsync(playlist);
            return response.Resource;
        }

        public async Task<Playlist> GetPlaylistByIdAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Playlist>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null; // Playlist not found
            }
        }

        public async Task<Playlist> UpdatePlaylistAsync(string id, Playlist playlist)
        {
            await _container.ReplaceItemAsync(playlist, id, new PartitionKey(id));
            return playlist;
        }

        public async Task<bool> DeletePlaylistAsync(string id)
        {
            try
            {
                await _container.DeleteItemAsync<Playlist>(id, new PartitionKey(id));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false; // Playlist not found
            }
        }

        public async Task<IEnumerable<Playlist>> GetAllPlaylistsAsync()
        {
            var query = _container.GetItemQueryIterator<Playlist>();
            var results = new List<Playlist>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }
    }
}
