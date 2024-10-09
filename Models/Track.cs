using System.Collections.Generic;

namespace MoviePlaylist.Models
{
    /// <summary>
    /// Represents a track within a playlist, consisting of multiple segments.
    /// </summary>
    public class Track
    {
        /// <summary>
        /// Gets or sets the unique identifier for the track.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the track.
        /// </summary>
        public string Title { get; set; }

        public double Distance { get; set; }

        /// <summary>
        /// Gets or sets the list of segments in the track.
        /// </summary>
        public List<Segment> Segments { get; set; }
    }
}
