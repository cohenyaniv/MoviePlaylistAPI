namespace MoviePlaylist.Models
{
    /// <summary>
    /// Represents a segment of a track, including time offset information.
    /// </summary>
    public class Segment
    {
        /// <summary>
        /// Gets or sets the left time locator (offset) in seconds.
        /// </summary>
        public double LeftTimeLocator { get; set; }

        /// <summary>
        /// Gets or sets the right time locator (offset) in seconds.
        /// </summary>
        public double RightTimeLocator { get; set; }
    }
}
