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
    }
}
