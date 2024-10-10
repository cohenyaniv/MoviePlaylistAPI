using MoviePlaylist.Models;
using Newtonsoft.Json;

namespace MoviePlaylist.Repositories
{
    /// <summary>
    /// Implementation of the IPlaylistRepository interface using CosmosDB.
    /// </summary>
    public class PlaylistRepository : IPlaylistRepository
    {
       
        private List<Playlist> _playlistList;

        public PlaylistRepository()
        {
          
            // this is for the mock playLists
            // Path to the JSON file
            string filePath = "ResourceMock/PlayLists.json";

            // Initialize mock list from JSON file
            _playlistList = InitializeListFromJson(filePath);
        }

        public async Task<Playlist> GetPlaylistByIdAsync(string id)
        {
            var result = _playlistList.FirstOrDefault(x => x.PlaylistId == id);
            return result;
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
