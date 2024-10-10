// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;

// namespace WebApi
// {
//     [Route("[controller]")]
//     public class PlaylistController : Controller
//     {
//         private readonly ILogger<PlaylistController> _logger;

//         public PlaylistController(ILogger<PlaylistController> logger)
//         {
//             _logger = logger;
//         }

//         public IActionResult Index()
//         {
//             return View();
//         }

//         [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//         public IActionResult Error()
//         {
//             return View("Error!");
//         }
//     }
// }



    
using Microsoft.AspNetCore.Mvc;
using MoviePlaylist.Models;
using MoviePlaylist.Services;
using System.Threading.Tasks;

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