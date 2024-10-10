using Microsoft.Azure.Cosmos;
using Models;
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
        private readonly QueueService _queueService;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IUserPlaylistRepository _userPlaylistRepository;

        /// <summary>
        /// Initializes a new instance of the PlaylistService with the required repository dependency.
        /// </summary>
        public PlaylistService(IPlaylistRepository playlistRepository, IUserPlaylistRepository userPlaylistRepository, QueueService queueService)
        {
            _playlistRepository = playlistRepository;
            _userPlaylistRepository = userPlaylistRepository;
            _queueService = queueService;
        }

        /// <summary>
        /// Creates a new playlist and assigns a unique ID and creation timestamp.
        /// </summary>
        public async Task<Playlist> CreatePlaylistAsync(Playlist playlist)
        {
            if (playlist == null)
                throw new ArgumentNullException(nameof(playlist));

            // Assign unique ID and creation timestamp
            playlist.PlaylistId = Guid.NewGuid().ToString();
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
            if (playlist == null || id != playlist.PlaylistId)
                throw new ArgumentException("Invalid playlist data.");

            var existingPlaylist = await _playlistRepository.GetPlaylistByIdAsync(id);
            if (existingPlaylist == null)
                throw new Exception("Playlist not found.");

            // Update properties
            existingPlaylist.Name = playlist.Name;
            existingPlaylist.Tracks = playlist.Tracks;

            return await _playlistRepository.UpdatePlaylistAsync(playlist.PlaylistId, existingPlaylist);
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
        public async Task<bool> StartPlaylistAsync(string playlistId, string userId)
        {
            var playlist = await _userPlaylistRepository.GetUserPlaylistAsync(userId);
            if (playlist == null)
                throw new Exception("Playlist not attached, please attach first.");

            if (playlist.Status == PlaylistStatus.Started)
                throw new Exception("The playlist already started.");

            // Logic to start playlist and initialize playback
            UserCurrentPlaylist userCurrentPlaylist = new UserCurrentPlaylist() { 
            CurrentPositionInSegment = 0,
            CurrentSegmentIndex = 0,
            CurrentTrackIndex = 0,
            PlaylistId = playlistId,
            UserId = userId,
            Status = PlaylistStatus.Started,
            LastStartedAt = DateTime.UtcNow
            };

            _queueService.QueueUserPlaylist(userCurrentPlaylist);
            //await _userPlaylistRepository.SaveUserPlaylistAsync(userCurrentPlaylist);
            return true;
        }

        /// <summary>
        /// Stops the playlist and records the current position and stop time.
        /// </summary>
        public async Task<bool> StopPlaylistAsync(string playlistId, string userId)
        {
            var playlist = await _playlistRepository.GetPlaylistByIdAsync(playlistId);
            if (playlist == null)
                throw new Exception("Playlist not found.");

            // Get the user position
            UserCurrentPlaylist userCurrentPlaylist = await _userPlaylistRepository.GetUserPlaylistAsync(userId);

            // Logic to stop playlist and save the current position
            userCurrentPlaylist.Status = PlaylistStatus.Stopped;
            userCurrentPlaylist.LastStartedAt = DateTime.UtcNow;

            _queueService.QueueUserPlaylist(userCurrentPlaylist);
            //await _userPlaylistRepository.SaveUserPlaylistAsync(userCurrentPlaylist);
            return true;
        }

        /// <summary>
        /// Resumes the playlist from the last stopped position.
        /// </summary>
        public async Task<bool> AttachPlaylistToUserAsync(string playlistId, string userId)
        {
            var playlist = await _playlistRepository.GetPlaylistByIdAsync(playlistId);

            if (playlist == null)
                throw new Exception("Playlist not found.");

            // Get the user position
            UserCurrentPlaylist userCurrentPlaylist = await _userPlaylistRepository.GetUserPlaylistAsync(userId);
            if (userCurrentPlaylist == null)
            {
                // Logic to start playlist and initialize playback
                userCurrentPlaylist = new UserCurrentPlaylist()
                {
                    CurrentPositionInSegment = 0,
                    CurrentSegmentIndex = 0,
                    CurrentTrackIndex = 0,
                    PlaylistId = playlistId,
                    UserId = userId,
                    Status = PlaylistStatus.Attached,
                    LastStartedAt = DateTime.UtcNow
                };

                _queueService.QueueUserPlaylist(userCurrentPlaylist);
            }
            return true;
        }

        public async Task<PlaylistProgress> GetPlaylistProgressAsync(string playlistId, string userId)
        {
            // Fetch the playlist by ID from the repository
            var playlist = await _playlistRepository.GetPlaylistByIdAsync(playlistId);

            if (playlist == null)
            {
                throw new Exception($"Playlist with ID {playlistId} not found.");
            }

            // Create and populate a new PlaylistProgress object
            var playlistProgress = new PlaylistProgress
            {
                PlaylistId = playlist.PlaylistId,
                TotalDistanceTraversed = CalculateTotalDistance(playlist),
                CurrentTrackIndex = GetCurrentTrackIndex(userId, playlist),
                CurrentTrackSegmentInfo = GetCurrentTrackSegmentInfo(userId, playlist),
                AdjacentTrackSegmentInfo = GetAdjacentTrackSegmentInfo(userId, playlist),
                CurrentTrackCompletionPercentage = CalculateTrackCompletion(userId, playlist),
                PlaylistCompletionPercentage = CalculatePlaylistCompletion(userId, playlist),
                InteractionHistories = GetInteractionHistory(userId, playlist),
                TotalDuration = CalculateTotalDuration(userId, playlist)
            };

            return playlistProgress;
        }

        private double CalculateDistanceTraversed(UserCurrentPlaylist userPlaylist, Playlist playlist)
        {
            return 1;
            // Sum up the length of segments in tracks that have been traversed
        }

        private double CalculateCurrentTrackPercentage(UserCurrentPlaylist userPlaylist, Playlist playlist)
        {
            return 1;
            // Calculate how much of the current track the user has completed
        }

        private double CalculateEntirePlaylistPercentage(UserCurrentPlaylist userPlaylist, Playlist playlist)
        {
            return 1;
            // Calculate how much of the entire playlist has been completed
        }

        private Track GetTrackInfo(UserCurrentPlaylist userPlaylist, Playlist playlist)
        {
            return null;
            // Provide information about the current track and segment
        }
        // Helper methods for calculating progress details
        private double CalculateTotalDistance(Playlist playlist)
        {
            // Calculate total distance traversed in the playlist (in kilometers)
            // Example: Assume each track has a defined distance, sum them up
            double totalDistance = 0;
            foreach (var track in playlist.Tracks)
            {
                totalDistance += track.Distance; // Assuming Track has a Distance property
            }
            return totalDistance;
        }

        private int GetCurrentTrackIndex(string userId, Playlist playlist)
        {
            // Logic to determine the current track index based on user progress
            return 0; // Example: Replace with actual logic
        }

        private TrackSegmentInfo GetCurrentTrackSegmentInfo(string userId, Playlist playlist)
        {
            // Logic to retrieve details about the current track segment the user is in
            return new TrackSegmentInfo(); // Example: Replace with actual logic
        }

        private TrackSegmentInfo GetAdjacentTrackSegmentInfo(string userId, Playlist playlist)
        {
            // Logic to retrieve details about the adjacent track segment
            return new TrackSegmentInfo(); // Example: Replace with actual logic
        }

        private double CalculateTrackCompletion(string userId, Playlist playlist)
        {
            // Logic to calculate the percentage of the current track completed
            return 0; // Example: Replace with actual logic
        }

        private double CalculatePlaylistCompletion(string userId, Playlist playlist)
        {
            // Logic to calculate the percentage of the entire playlist completed
            return 0; // Example: Replace with actual logic
        }

        private List<InteractionHistory> GetInteractionHistory(string userId, Playlist playlist)
        {
            // Logic to retrieve the user's interaction history with the playlist
            return new List<InteractionHistory>(); // Example: Replace with actual logic
        }

        private TimeSpan CalculateTotalDuration(string userId, Playlist playlist)
        {
            // Logic to calculate the total time spent on the playlist
            return TimeSpan.Zero; // Example: Replace with actual logic
        }

    }
}
