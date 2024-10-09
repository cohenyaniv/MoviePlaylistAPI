using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MoviePlaylist.Models
{
    /// <summary>
    /// Represents a playlist containing multiple tracks.
    /// </summary>
    public class Playlist
    {
        /// <summary>
        /// Gets or sets the unique identifier for the playlist.
        /// </summary>
        [JsonProperty("id")]
        public string PlaylistId { get; set; }

        /// <summary>
        /// Gets or sets the name of the playlist.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of tracks in the playlist.
        /// </summary>
        public List<Track> Tracks { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the playlist was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        
    }
}
