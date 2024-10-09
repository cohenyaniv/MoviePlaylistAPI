using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using MoviePlaylist.Models;
using Newtonsoft.Json;

namespace MoviePlaylist.Repositories
{
    /// <summary>
    /// Implementation of the IPlaylistRepository interface using CosmosDB.
    /// </summary>
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Microsoft.Azure.Cosmos.Container _container;

        private List<Playlist> _playlistList;

        public PlaylistRepository(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(databaseName, containerName);

            // this is for the mock playLists
            // Path to the JSON file
            string filePath = "ResourceMock/PlayLists.json";

            // Initialize mock list from JSON file
            _playlistList = InitializeListFromJson(filePath);
        }

        public async Task<Playlist> AddPlaylistAsync(Playlist playlist)
        {
            var response = await _container.CreateItemAsync(playlist);
            return response.Resource;
        }

        public async Task<Playlist> GetPlaylistByIdAsync(string id)
        {
            var result = _playlistList.FirstOrDefault(x => x.PlaylistId == id);
            return result;
            //try
            //{
            //    var response = await _container.ReadItemAsync<Playlist>(id, PartitionKey.None);
            //    return response.Resource;
            //}
            //catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            //{
            //    return null; // Playlist not found
            //}
        }

        public async Task<Playlist> UpdatePlaylistAsync(string id, Playlist playlist)
        {
            await _container.ReplaceItemAsync(playlist, id, PartitionKey.None);
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

        // Method to initialize a list from a JSON file
        private static List<Playlist> InitializeListFromJson(string filePath)
        {
            // Read the JSON file
            var jsonString = File.ReadAllText(filePath);

            // Deserialize the JSON string into a list of Track objects
            return JsonConvert.DeserializeObject<List<Playlist>>(jsonString);
        }
    }
}
