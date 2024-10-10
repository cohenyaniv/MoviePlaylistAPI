using Models;

namespace MoviePlaylist.Repositories
{
    /// <summary>
    /// This interface responsible for saving the user action in the blob
    /// </summary>
    public interface IUserHistoryRepository
    {
        /// <summary>
        /// Save the user current status in the history blob
        /// </summary>
        /// <param name="userPlaylist"></param>
        Task SaveUserPlaylistAsync(UserCurrentPlaylist userPlaylist);

        Task<string> GetUserHistoryAsync(string userId);
    }
}
