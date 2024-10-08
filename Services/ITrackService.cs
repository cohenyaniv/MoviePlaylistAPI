using System.Threading.Tasks;
using MoviePlaylist.Models;
using MoviePlaylist.Models;

namespace MoviePlaylist.Services
{
    /// <summary>
    /// Interface for track-related business logic.
    /// </summary>
    public interface ITrackService
    {
        Task<Track> AddTrackToPlaylistAsync(string playlistId, Track track);
        Task<Track> GetTrackFromPlaylistAsync(string playlistId, string trackId);
        Task<Track> UpdateTrackInPlaylistAsync(string playlistId, string trackId, Track track);
        Task<bool> DeleteTrackFromPlaylistAsync(string playlistId, string trackId);
    }
}
