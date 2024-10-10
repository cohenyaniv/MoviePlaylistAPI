//using Microsoft.AspNetCore.Mvc;
//using MoviePlaylist.Models;
//using MoviePlaylist.Services;
//using System.Threading.Tasks;

//namespace MoviePlaylist.Controllers
//{
//    /// <summary>
//    /// API controller to manage tracks within playlists. Handles HTTP requests for track operations.
//    /// </summary>
//    [ApiController]
//    [Route("api/[controller]")]
//    public class TrackController : ControllerBase
//    {
//        private readonly ITrackService _trackService;

//        /// <summary>
//        /// Initializes a new instance of the TrackController with the required track service.
//        /// </summary>
//        public TrackController(ITrackService trackService)
//        {
//            _trackService = trackService;
//        }

//        /// <summary>
//        /// Adds a new track to the specified playlist.
//        /// </summary>
//        /// <param name="playlistId">The ID of the playlist to which the track will be added.</param>
//        /// <param name="track">The track details to be added.</param>
//        [HttpPost("{playlistId}/tracks")]
//        public async Task<IActionResult> AddTrack(string playlistId, [FromBody] Track track)
//        {
//            if (track == null || !ModelState.IsValid)
//            {
//                return BadRequest("Invalid track data.");
//            }

//            var createdTrack = await _trackService.AddTrackToPlaylistAsync(playlistId, track);
//            return CreatedAtAction(nameof(GetTrackById), new { playlistId = playlistId, trackId = createdTrack.Id }, createdTrack);
//        }

//        /// <summary>
//        /// Retrieves a specific track from a playlist by its ID.
//        /// </summary>
//        /// <param name="playlistId">The ID of the playlist containing the track.</param>
//        /// <param name="trackId">The ID of the track to retrieve.</param>
//        [HttpGet("{playlistId}/tracks/{trackId}")]
//        public async Task<IActionResult> GetTrackById(string playlistId, string trackId)
//        {
//            var track = await _trackService.GetTrackFromPlaylistAsync(playlistId, trackId);
//            if (track == null)
//            {
//                return NotFound();
//            }
//            return Ok(track);
//        }

//        /// <summary>
//        /// Updates an existing track within a specified playlist.
//        /// </summary>
//        /// <param name="playlistId">The ID of the playlist containing the track.</param>
//        /// <param name="trackId">The ID of the track to update.</param>
//        /// <param name="track">The updated track details.</param>
//        [HttpPut("{playlistId}/tracks/{trackId}")]
//        public async Task<IActionResult> UpdateTrack(string playlistId, string trackId, [FromBody] Track track)
//        {
//            if (track == null || trackId != track.Id || !ModelState.IsValid)
//            {
//                return BadRequest("Invalid track data.");
//            }

//            var updatedTrack = await _trackService.UpdateTrackInPlaylistAsync(playlistId, trackId, track);
//            if (updatedTrack == null)
//            {
//                return NotFound();
//            }
//            return Ok(updatedTrack);
//        }

//        /// <summary>
//        /// Deletes a specific track from a playlist by its ID.
//        /// </summary>
//        /// <param name="playlistId">The ID of the playlist containing the track.</param>
//        /// <param name="trackId">The ID of the track to delete.</param>
//        [HttpDelete("{playlistId}/tracks/{trackId}")]
//        public async Task<IActionResult> DeleteTrack(string playlistId, string trackId)
//        {
//            var result = await _trackService.DeleteTrackFromPlaylistAsync(playlistId, trackId);
//            if (!result)
//            {
//                return NotFound();
//            }
//            return NoContent();
//        }
//    }
//}