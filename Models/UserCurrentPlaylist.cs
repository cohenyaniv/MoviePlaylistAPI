using MoviePlaylist.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UserCurrentPlaylist
    {
        /// <summary>
        /// Gets or sets the unique identifier for the playlist.
        /// </summary>
        [JsonProperty("id")]
        public string UserId { get; set; }
        public string Id { get; set; }
        public string PlaylistId { get; set; }
        public int CurrentTrackIndex { get; set; }
        public int CurrentSegmentIndex { get; set; }
        public double CurrentPositionInSegment { get; set; } // In seconds or kilometers
        /// <summary>
        /// Gets or sets the status of the playlist (e.g., Started, Stopped).
        /// </summary>
        public PlaylistStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the playlist was last started.
        /// </summary>
        public DateTime? LastStartedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the playlist was last stopped.
        /// </summary>
        public DateTime? LastStoppedAt { get; set; }
        //public List<UserInteraction> Interactions { get; set; } = new List<UserInteraction>();
    }
}
