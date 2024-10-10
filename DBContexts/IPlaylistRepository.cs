using System.Collections.Generic;
using System.Threading.Tasks;
using MoviePlaylist.Models;

namespace MoviePlaylist.Repositories
{
    /// <summary>
    /// Interface for managing playlist data operations.
    /// </summary>
    public interface IPlaylistRepository
    {
        /// <summary>
        /// Adds a new playlist to the data store.
        /// </summary>
        /// <param name="playlist">The playlist to add.</param>
        /// <returns>The created playlist.</returns>
        //Task<Playlist> AddPlaylistAsync(Playlist playlist);

        /// <summary>
        /// Retrieves a playlist by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the playlist.</param>
        /// <returns>The retrieved playlist, or null if not found.</returns>
        Task<Playlist> GetPlaylistByIdAsync(string id);

        /// <summary>
        /// Updates an existing playlist in the data store.
        /// </summary>
        /// <param name="id">The unique identifier of the playlist to update.</param>
        /// <param name="playlist">The updated playlist details.</param>
        /// <returns>The updated playlist.</returns>
        //Task<Playlist> UpdatePlaylistAsync(string id, Playlist playlist);

        /// <summary>
        /// Deletes a playlist by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the playlist to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        //Task<bool> DeletePlaylistAsync(string id);

        /// <summary>
        /// Retrieves all playlists from the data store.
        /// </summary>
        /// <returns>A list of all playlists.</returns>
        //Task<IEnumerable<Playlist>> GetAllPlaylistsAsync();
    }
}
