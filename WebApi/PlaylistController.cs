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
    public class PlaylistController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        /// <summary>
        /// Initializes a new instance of the PlaylistController with the required playlist service.
        /// </summary>
        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        /// <summary>
        /// Creates a new playlist.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromBody] Playlist playlist)
        {
            if (playlist == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid playlist data.");
            }

            var createdPlaylist = await _playlistService.CreatePlaylistAsync(playlist);
            return CreatedAtAction(nameof(GetPlaylistById), new { id = createdPlaylist.Id }, createdPlaylist);
        }

        /// <summary>
        /// Retrieves a playlist by its ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlaylistById(string id)
        {
            var playlist = await _playlistService.GetPlaylistByIdAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }
            return Ok(playlist);
        }

        /// <summary>
        /// Updates an existing playlist.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlaylist(string id, [FromBody] Playlist playlist)
        {
            if (playlist == null || id != playlist.Id || !ModelState.IsValid)
            {
                return BadRequest("Invalid playlist data.");
            }

            var updatedPlaylist = await _playlistService.UpdatePlaylistAsync(id, playlist);
            if (updatedPlaylist == null)
            {
                return NotFound();
            }
            return Ok(updatedPlaylist);
        }

        /// <summary>
        /// Deletes a playlist by its ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(string id)
        {
            var result = await _playlistService.DeletePlaylistAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Starts a specific playlist for a user.
        /// </summary>
        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartPlaylist(string id)
        {
            var result = await _playlistService.StartPlaylistAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // Additional endpoints can be added here as needed...
    }
}