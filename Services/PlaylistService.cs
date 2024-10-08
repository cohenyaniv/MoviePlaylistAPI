using MoviePlaylist.Models;
using MoviePlaylist.Repositories;
using System;
using System.Threading.Tasks;

namespace MoviePlaylist.Services
{
    /// <summary>
    /// Implements business logic for managing playlists and tracks.
    /// </summary>
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _playlistRepository;

        /// <summary>
        /// Initializes a new instance of the PlaylistService with the required repository dependency.
        /// </summary>
        public PlaylistService(IPlaylistRepository playlistRepository)
        {
            _playlistRepository = playlistRepository;
        }

        /// <summary>
        /// Creates a new playlist and assigns a unique ID and creation timestamp.
        /// </summary>
        public async Task<Playlist> CreatePlaylistAsync(Playlist playlist)
        {
            if (playlist == null)
                throw new ArgumentNullException(nameof(playlist));

            // Assign unique ID and creation timestamp
            playlist.Id = Guid.NewGuid().ToString();
            playlist.CreatedAt = DateTime.UtcNow;

            return await _playlistRepository.AddPlaylistAsync(playlist);
        }

        /// <summary>
        /// Retrieves a playlist by its ID.
        /// </summary>
        public async Task<Playlist> GetPlaylistByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Playlist ID cannot be null or empty.");

            return await _playlistRepository.GetPlaylistByIdAsync(id);
        }

        /// <summary>
        /// Updates an existing playlist.
        /// </summary>
        public async Task<Playlist> UpdatePlaylistAsync(string id, Playlist playlist)
        {
            if (playlist == null || id != playlist.Id)
                throw new ArgumentException("Invalid playlist data.");

            var existingPlaylist = await _playlistRepository.GetPlaylistByIdAsync(id);
            if (existingPlaylist == null)
                throw new Exception("Playlist not found.");

            // Update properties
            existingPlaylist.Name = playlist.Name;
            existingPlaylist.Tracks = playlist.Tracks;

            return await _playlistRepository.UpdatePlaylistAsync(playlist.Id, existingPlaylist);
        }

        /// <summary>
        /// Deletes a playlist by its ID.
        /// </summary>
        public async Task<bool> DeletePlaylistAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Playlist ID cannot be null or empty.");

            return await _playlistRepository.DeletePlaylistAsync(id);
        }

        /// <summary>
        /// Starts a playlist, setting its status to 'Started' and recording the start time.
        /// </summary>
        public async Task<bool> StartPlaylistAsync(string playlistId)
        {
            var playlist = await _playlistRepository.GetPlaylistByIdAsync(playlistId);
            if (playlist == null)
                throw new Exception("Playlist not found.");

            // Logic to start playlist and initialize playback
            playlist.Status = PlaylistStatus.Started;
            playlist.LastStartedAt = DateTime.UtcNow;

            await _playlistRepository.UpdatePlaylistAsync(playlist.Id, playlist);
            return true;
        }

        /// <summary>
        /// Stops the playlist and records the current position and stop time.
        /// </summary>
        public async Task<bool> StopPlaylistAsync(string playlistId)
        {
            var playlist = await _playlistRepository.GetPlaylistByIdAsync(playlistId);
            if (playlist == null)
                throw new Exception("Playlist not found.");

            // Logic to stop playlist and save the current position
            playlist.Status = PlaylistStatus.Stopped;
            playlist.LastStoppedAt = DateTime.UtcNow;

            await _playlistRepository.UpdatePlaylistAsync(playlist.Id, playlist);
            return true;
        }

        /// <summary>
        /// Resumes the playlist from the last stopped position.
        /// </summary>
        public async Task<bool> ContinuePlaylistAsync(string playlistId)
        {
            var playlist = await _playlistRepository.GetPlaylistByIdAsync(playlistId);
            if (playlist == null)
                throw new Exception("Playlist not found.");

            if (playlist.Status != PlaylistStatus.Stopped)
                throw new Exception("Cannot continue a playlist that hasn't been stopped.");

            // Logic to continue the playlist from the last position
            playlist.Status = PlaylistStatus.Continued;
            playlist.LastStartedAt = DateTime.UtcNow;

            await _playlistRepository.UpdatePlaylistAsync(playlist.Id, playlist);
            return true;
        }
    }
}
