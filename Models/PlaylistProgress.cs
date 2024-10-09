using System;
using System.Collections.Generic;

namespace MoviePlaylist.Models
{
    /// <summary>
    /// Represents the progress of a playlist for a user, including the current track and segment details,
    /// the position in the playlist, and the history of user interactions.
    /// </summary>
    public class PlaylistProgress
    {
        /// <summary>
        /// The unique ID of the playlist.
        /// </summary>
        public string PlaylistId { get; set; }

        /// <summary>
        /// The total distance traversed in the playlist, in kilometers.
        /// </summary>
        public double TotalDistanceTraversed { get; set; }

        /// <summary>
        /// The percentage of the current track that has been completed.
        /// </summary>
        public double CurrentTrackCompletionPercentage { get; set; }

        /// <summary>
        /// The percentage of the entire playlist that has been completed.
        /// </summary>
        public double PlaylistCompletionPercentage { get; set; }

        /// <summary>
        /// The index of the current track the user is on.
        /// </summary>
        public int CurrentTrackIndex { get; set; }

        /// <summary>
        /// The current segment index within the track that the user is on.
        /// </summary>
        public int CurrentSegmentIndex { get; set; }

        /// <summary>
        /// The position in seconds where the user last stopped in the current track.
        /// </summary>
        public double LastPositionInTrack { get; set; }

        /// <summary>
        /// The history of user interactions with the playlist (e.g., stops and starts).
        /// </summary>
        public List<InteractionHistory> InteractionHistories { get; set; }

        /// <summary>
        /// The total time spent on the playlist across all user interactions (in hh:mm:ss format).
        /// </summary>
        public TimeSpan TotalDuration { get; set; }

        /// <summary>
        /// Information about the current track and segment.
        /// </summary>
        public TrackSegmentInfo CurrentTrackSegmentInfo { get; set; }

        /// <summary>
        /// Information about the adjacent track and segment.
        /// </summary>
        public TrackSegmentInfo AdjacentTrackSegmentInfo { get; set; }
    }

    /// <summary>
    /// Represents the interaction history for the playlist.
    /// </summary>
    public class InteractionHistory
    {
        /// <summary>
        /// The time the user stopped the playlist.
        /// </summary>
        public DateTime StoppedAt { get; set; }

        /// <summary>
        /// The time the user started the playlist again.
        /// </summary>
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// The position in the playlist (in seconds) when the user stopped.
        /// </summary>
        public double PositionAtStop { get; set; }

        /// <summary>
        /// The duration for which the playlist was playing during this interaction.
        /// </summary>
        public TimeSpan Duration => StartedAt != default && StoppedAt != default
            ? StoppedAt - StartedAt
            : TimeSpan.Zero;
    }

    /// <summary>
    /// Provides information about the track and segment that the user is currently in.
    /// </summary>
    public class TrackSegmentInfo
    {
        /// <summary>
        /// The title of the current track.
        /// </summary>
        public string TrackTitle { get; set; }

        /// <summary>
        /// The segment number within the track.
        /// </summary>
        public int SegmentNumber { get; set; }

        /// <summary>
        /// The left time locator of the current segment (in seconds).
        /// </summary>
        public double LeftTimeLocator { get; set; }

        /// <summary>
        /// The right time locator of the current segment (in seconds).
        /// </summary>
        public double RightTimeLocator { get; set; }

        /// <summary>
        /// The total duration of the current segment (in seconds).
        /// </summary>
        public double SegmentDuration => RightTimeLocator - LeftTimeLocator;
    }
}
