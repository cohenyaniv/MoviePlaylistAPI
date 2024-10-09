namespace MoviePlaylist.Models
{
    /// <summary>
    /// Represents the possible statuses of a playlist.
    /// </summary>
    public enum PlaylistStatus
    {
        /// <summary>
        /// Indicates that the playlist is currently being played.
        /// </summary>
        Started,

        /// <summary>
        /// Indicates that the playlist is currently stopped.
        /// </summary>
        Stopped,

        /// <summary>
        /// Indicates that the playlist is currently attached to the user.
        /// </summary>
        Attached
    }
}
