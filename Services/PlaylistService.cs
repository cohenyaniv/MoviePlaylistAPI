using Models;
using MoviePlaylist.Models;
using MoviePlaylist.Repositories;

namespace MoviePlaylist.Services
{
    /// <summary>
    /// Implements business logic for managing playlists and tracks.
    /// </summary>
    public class PlaylistService : IPlaylistService
    {
        private readonly IQueueService _queueService;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IUserPlaylistRepository _userPlaylistRepository;
        private readonly IUserHistoryRepository _userHistoryRepository;
        private readonly UserCounterService _userCounterService;

        /// <summary>
        /// Initializes a new instance of the PlaylistService with the required repository dependency.
        /// </summary>
        public PlaylistService(IPlaylistRepository playlistRepository, IUserPlaylistRepository userPlaylistRepository, IQueueService queueService, UserCounterService userCounterService, IUserHistoryRepository userHistoryRepository)
        {
            _playlistRepository = playlistRepository;
            _userPlaylistRepository = userPlaylistRepository;
            _queueService = queueService;
            _userCounterService = userCounterService;
            _userHistoryRepository = userHistoryRepository;
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
            _userCounterService.StartCounter(userId);

            _queueService.QueueUserPlaylist(playlist);
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

            // Stop the counter and get how many seconds passed
            var runningSeconds = _userCounterService.GetCounterValue(userId);
            _userCounterService.StopCounter(userId);

            // Logic to stop playlist and save the current position
            userCurrentPlaylist.Status = PlaylistStatus.Stopped;
            userCurrentPlaylist.LastStoppedAt = userCurrentPlaylist.LastStoppedAt.Value.AddSeconds(runningSeconds);
            var totalDuration = userCurrentPlaylist.LastStoppedAt - userCurrentPlaylist.LastStartedAt;
            
            SetUserPositionInPlayList(playlist, totalDuration.Value.TotalSeconds, userCurrentPlaylist);
            _queueService.QueueUserPlaylist(userCurrentPlaylist);
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
            // We are updating in case there is no playlist (new enter) or changed playlist
            if (userCurrentPlaylist == null || userCurrentPlaylist.PlaylistId != playlistId)
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

        public async Task<PlaylistProgress> GetPlaylistProgressAsync(string userId)
        {
            // Fetch the playlist by ID from the repository
            var userPlaylistDetails = await _userPlaylistRepository.GetUserPlaylistAsync(userId);

            if (userPlaylistDetails == null)
            {
                throw new Exception($"User {userId} does not have current playlist.");
            }

            // Get the playList content
            var playList = await _playlistRepository.GetPlaylistByIdAsync(userPlaylistDetails.PlaylistId);

            if (playList == null)
            {
                throw new Exception($"Unable to find metadata for playlist {userPlaylistDetails.PlaylistId}.");
            }

            // Create and populate a new PlaylistProgress object
            var playlistProgress = new PlaylistProgress
            {

                PlaylistId = userPlaylistDetails.PlaylistId,
                
                CurrentTrackIndex = userPlaylistDetails.CurrentTrackIndex,
                CurrentSegmentIndex = userPlaylistDetails.CurrentSegmentIndex,
                TotalDistance = CalculateTotalDistance(playList),
                CurrentTrackSegmentInfo = GetCurrentTrackSegmentInfo(userPlaylistDetails, playList),
                AdjacentTrackSegmentInfo = GetAdjacentTrackSegmentInfo(userPlaylistDetails, playList),
                CurrentTrackCompletionPercentage = CalculateTrackCompletion(userPlaylistDetails, playList),
                PlaylistCompletionPercentage = CalculatePlaylistCompletion(userPlaylistDetails, playList),
                InteractionHistories = await _userHistoryRepository.GetUserHistoryAsync(userId),
                TotalDuration = CalculateTotalDuration(userPlaylistDetails, playList)
            };

            return playlistProgress;
        }

        private void SetUserPositionInPlayList(Playlist playList, double totalDurationInSeconds, UserCurrentPlaylist userCurrentPlaylist) {

            int duration = 0;
            int trackCounter = 0;
            int segmentCounter;
            // Get the duration from the start to the last stop

            foreach (Track track in playList.Tracks)
            {
                segmentCounter = 0;
                trackCounter = trackCounter + 1;
                foreach (Segment segment in track.Segments)
                {
                    segmentCounter = segmentCounter + 1;
                    if (totalDurationInSeconds > segment.LeftTimeLocator && totalDurationInSeconds < segment.RightTimeLocator)
                    {
                        userCurrentPlaylist.CurrentTrackIndex = trackCounter;
                        userCurrentPlaylist.CurrentSegmentIndex = segmentCounter;
                        userCurrentPlaylist.CurrentPositionInSegment = segment.LeftTimeLocator + totalDurationInSeconds;
                        return;
                    }
                }
            }
        }
       
        // Helper methods for calculating progress details
        private double CalculateTotalDistance(Playlist playlist)
        {
            // Calculate total distance traversed in the playlist (in kilometers)
            double totalDistance = 0;
            foreach (var track in playlist.Tracks)
            {
                totalDistance += track.Distance; // Assuming Track has a Distance property
            }
            return totalDistance;
        }

        /// <summary>
        /// Get the next Track/Segment information 
        /// </summary>
        /// <param name="userPlaylistDetails"></param>
        /// <param name="playlist"></param>
        /// <returns>The information of the next segment and track</returns>
        private TrackSegmentInfo GetAdjacentTrackSegmentInfo(UserCurrentPlaylist userPlaylistDetails, Playlist playlist)
        {
            TrackSegmentInfo AdjacentTrackSegmentInfo = new TrackSegmentInfo();
            if (playlist.Tracks[userPlaylistDetails.CurrentTrackIndex-1].Segments.Count >= userPlaylistDetails.CurrentSegmentIndex)
            {
                AdjacentTrackSegmentInfo.TrackTitle = playlist.Tracks[userPlaylistDetails.CurrentTrackIndex-1].Title;
                AdjacentTrackSegmentInfo.SegmentNumber = userPlaylistDetails.CurrentSegmentIndex;
            }
            else // The next segment is on the next track (if exist)
            {
                if (playlist.Tracks.Count >= userPlaylistDetails.CurrentTrackIndex)
                {
                    AdjacentTrackSegmentInfo.TrackTitle = playlist.Tracks[userPlaylistDetails.CurrentTrackIndex].Title;
                    AdjacentTrackSegmentInfo.SegmentNumber = 1;
                }
            }
            return AdjacentTrackSegmentInfo;
        }

        private TrackSegmentInfo GetCurrentTrackSegmentInfo(UserCurrentPlaylist userPlaylistDetails, Playlist playlist)
        {
            TrackSegmentInfo CurrentTrackSegmentInfo = new TrackSegmentInfo() { 
                TrackTitle = playlist.Tracks[userPlaylistDetails.CurrentTrackIndex-1].Title,
                SegmentNumber = userPlaylistDetails.CurrentSegmentIndex
            };

            return CurrentTrackSegmentInfo;
        }

        private double CalculateTrackCompletion(UserCurrentPlaylist userPlaylistDetails, Playlist playlist)
        {
            double totalTracksDurationAchived = 0;
            for (int i = 0; i < userPlaylistDetails.CurrentTrackIndex; i++)
            {
                totalTracksDurationAchived += playlist.Tracks[i].Segments.Last<Segment>().RightTimeLocator;
            }

            return userPlaylistDetails.CurrentPositionInSegment / totalTracksDurationAchived * 100;
        }

        private double CalculatePlaylistCompletion(UserCurrentPlaylist userPlaylistDetails, Playlist playlist)
        {
            double totalTracksDuration = 0;
            // Get the left offset of the current track/segment
            double totalLengthOfTrack = playlist.Tracks.Last<Track>().Segments.Last<Segment>().RightTimeLocator;
            // Summerize duration of all tracks
            foreach (var track in playlist.Tracks) {
                totalTracksDuration += track.Segments.Last<Segment>().RightTimeLocator;
            }
            return userPlaylistDetails.CurrentPositionInSegment / totalTracksDuration * 100;
        }

        private string CalculateTotalDuration(UserCurrentPlaylist userPlaylistDetails, Playlist playlist)
        {
            // Create a TimeSpan from the total seconds
            TimeSpan timeSpan = TimeSpan.FromSeconds(userPlaylistDetails.CurrentPositionInSegment);

            // Format the TimeSpan to HH:mm:ss
            string formattedTime = timeSpan.ToString(@"hh\:mm\:ss");

            return formattedTime;
        }

    }
}
