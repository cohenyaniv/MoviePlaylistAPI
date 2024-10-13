using Microsoft.AspNetCore.Mvc;
using MoviePlaylist.Services;

namespace WebApi.Controllers
{
    /// <summary>
    /// API controller to manage playlists. Handles HTTP requests for playlist operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserPlaylistController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        /// <summary>
        /// Initializes a new instance of the PlaylistController with the required playlist service.
        /// </summary>
        public UserPlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpPost("start/{playlistId}/{userId}")]
        public async Task<IActionResult> StartPlaylist(string playlistId, string userId)
        {
            var userPlaylist = await _playlistService.StartPlaylistAsync(playlistId, userId);
            return Ok(userPlaylist);
        }

        [HttpPost("stop/{playlistId}/{userId}")]
        public async Task<IActionResult> StopPlaylist(string playlistId, string userId)
        {
            var result = await _playlistService.StopPlaylistAsync(playlistId, userId);
            return Ok(result);
        }

        [HttpPost("attach/{playlistId}/{userId}")]
        public async Task<IActionResult> AttachPlaylistToUser(string playlistId, string userId)
        {
            var result = await _playlistService.AttachPlaylistToUserAsync(playlistId, userId);
            return Ok(result);
        }

        [HttpGet("progress/{userId}")]
        public async Task<IActionResult> GetPlaylistProgress(string userId)
        {
            var progress = await _playlistService.GetPlaylistProgressAsync(userId);
            return Ok(progress);
        }

    }
}