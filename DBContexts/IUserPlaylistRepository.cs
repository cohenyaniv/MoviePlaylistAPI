using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using MoviePlaylist.Models;

namespace MoviePlaylist.Repositories
{
    /// <summary>
    /// Interface for managing user-specific playlist data such as progress and interaction history.
    /// </summary>
    public interface IUserPlaylistRepository
    {
        /// <summary>
        /// Retrieves the user-specific playlist progress by user ID and playlist ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <returns>A task that represents the asynchronous operation, containing the playlist progress.</returns>
        Task<UserCurrentPlaylist> GetUserPlaylistAsync(string userId);

        Task AddPlaylistAsync(UserCurrentPlaylist playlist);

        /// <summary>
        /// Saves or updates the user-specific playlist progress.
        /// </summary>
        /// <param name="userPlaylist">The user's playlist progress object to save.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        Task SaveUserPlaylistAsync(UserCurrentPlaylist userPlaylist);

        /// <summary>
        /// Retrieves the interaction history for a user in a specific playlist.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="playlistId">The ID of the playlist.</param>
        /// <returns>A task representing the asynchronous operation, containing a list of interaction history records.</returns>
        //Task<List<InteractionHistory>> GetInteractionHistoryAsync(string userId, string playlistId);
    }
}
