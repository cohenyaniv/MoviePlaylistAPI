using System.Collections.Generic;
using System.Threading.Tasks;
using MoviePlaylist.API.Models;

namespace MoviePlaylist.Services
{
    /// <summary>
    /// Service to manage track-related operations for playlists.
    /// </summary>
    public class TrackService : ITrackService
    {
        private readonly IPlaylistRepository _playlistRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackService"/> with the specified playlist repository.
        /// </summary>
        public TrackService(IPlaylistRepository playlistRepository)
        {
            _playlistRepository = playlistRepository;
        }

        /// <summary>
        /// Adds a new track to the specified playlist.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist to which the track will be added.</param>
        /// <param name="track">The track details to be added.</param>
        /// <returns>The created track.</returns>
        public async Task<Track> AddTrackToPlaylistAsync(string playlistId, Track track)
        {
            var playlist = await _playlistRepository.GetPlaylistByIdAsync(playlistId);

            if (playlist == null)
            {
                return null; // or throw an exception, depending on how you handle errors.
            }

            track.Id = Guid.NewGuid().ToString();
            playlist.Tracks.Add(track);

            await _playlistRepository.UpdatePlaylistAsync(playlistId, playlist);
            return track;
        }

        /// <summary>
        /// Retrieves a specific track from the specified playlist.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist containing the track.</param>
        /// <param name="trackId">The ID of the track to retrieve.</param>
        /// <returns>The retrieved track, or null if not found.</returns>
        public async Task<Track> GetTrackFromPlaylistAsync(string playlistId, string trackId)
        {
            var playlist = await _playlistRepository.GetPlaylistByIdAsync(playlistId);
            return playlist?.Tracks.Find(t => t.Id == trackId);
        }

        /// <summary>
        /// Updates an existing track in the specified playlist.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist containing the track.</param>
        /// <param name="trackId">The ID of the track to update.</param>
        /// <param name="track">The updated track details.</param>
        /// <returns>The updated track, or null if not found.</returns>
        public async Task<Track> UpdateTrackInPlaylistAsync(string playlistId, string trackId, Track track)
        {
            var playlist = await _playlistRepository.GetPlaylistByIdAsync(playlistId);
            if (playlist == null)
            {
                return null;
            }

            var existingTrack = playlist.Tracks.Find(t => t.Id == trackId);
            if (existingTrack == null)
            {
                return null;
            }

            // Update track details.
            existingTrack.Title = track.Title;
            existingTrack.Segments = track.Segments;

            await _playlistRepository.UpdatePlaylistAsync(playlistId, playlist);
            return existingTrack;
        }

        /// <summary>
        /// Deletes a track from the specified playlist.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist containing the track.</param>
        /// <param name="trackId">The ID of the track to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        public async Task<bool> DeleteTrackFromPlaylistAsync(string playlistId, string trackId)
        {
            var playlist = await _playlistRepository.GetPlaylistByIdAsync(playlistId);
            if (playlist == null)
            {
                return false;
            }

            var trackToRemove = playlist.Tracks.Find(t => t.Id == trackId);
            if (trackToRemove == null)
            {
                return false;
            }

            playlist.Tracks.Remove(trackToRemove);
            await _playlistRepository.UpdatePlaylistAsync(playlistId, playlist);
            return true;
        }
    }
}
