using MoviePlaylist.Models;
using System.Threading.Tasks;

namespace MoviePlaylist.Services
{
    /// <summary>
    /// Defines the contract for the Playlist service which manages business logic for playlists.
    /// </summary>
    public interface IPlaylistService
    {
        /// <summary>
        /// Creates a new playlist in the system.
        /// </summary>
        Task<Playlist> CreatePlaylistAsync(Playlist playlist);

        /// <summary>
        /// Retrieves a playlist by its unique ID.
        /// </summary>
        Task<Playlist> GetPlaylistByIdAsync(string id);

        /// <summary>
        /// Updates an existing playlist.
        /// </summary>
        Task<Playlist> UpdatePlaylistAsync(string id, Playlist playlist);

        /// <summary>
        /// Deletes a playlist by its ID.
        /// </summary>
        Task<bool> DeletePlaylistAsync(string id);

        /// <summary>
        /// Starts a playlist and initializes playback for the user.
        /// </summary>
        Task<bool> StartPlaylistAsync(string playlistId);

        /// <summary>
        /// Stops the current playlist and records the playback position.
        /// </summary>
        Task<bool> StopPlaylistAsync(string playlistId);

        /// <summary>
        /// Continues a playlist from where it was last stopped.
        /// </summary>
        Task<bool> ContinuePlaylistAsync(string playlistId);
    }
}
