using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using MoviePlaylist.Models;

namespace MoviePlaylist.Repositories
{
    /// <summary>
    /// Implementation of the ITrackRepository interface using CosmosDB.
    /// </summary>
    public class TrackRepository : ITrackRepository
    {
        private readonly Microsoft.Azure.Cosmos.Container _container;

        public TrackRepository(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            _container = cosmosClient.GetContainer(databaseName, containerName);
        }

        public async Task<Track> AddTrackAsync(Track track)
        {
            var response = await _container.CreateItemAsync(track);
            return response.Resource;
        }

        public async Task<Track> GetTrackByIdAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Track>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null; // Track not found
            }
        }

        public async Task<Track> UpdateTrackAsync(string id, Track track)
        {
            await _container.ReplaceItemAsync(track, id, new PartitionKey(id));
            return track;
        }

        public async Task<bool> DeleteTrackAsync(string id)
        {
            try
            {
                await _container.DeleteItemAsync<Track>(id, new PartitionKey(id));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false; // Track not found
            }
        }

        public async Task<IEnumerable<Track>> GetAllTracksAsync()
        {
            var query = _container.GetItemQueryIterator<Track>();
            var results = new List<Track>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }
    }
}
