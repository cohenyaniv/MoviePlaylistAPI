using System.Collections.Generic;
using System.Threading.Tasks;
using MoviePlaylist.Models;

namespace MoviePlaylist.Repositories
{
    /// <summary>
    /// Interface for managing track data operations.
    /// </summary>
    public interface ITrackRepository
    {
        /// <summary>
        /// Adds a new track to the data store.
        /// </summary>
        /// <param name="track">The track to add.</param>
        /// <returns>The created track.</returns>
        Task<Track> AddTrackAsync(Track track);

        /// <summary>
        /// Retrieves a track by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the track.</param>
        /// <returns>The retrieved track, or null if not found.</returns>
        Task<Track> GetTrackByIdAsync(string id);

        /// <summary>
        /// Updates an existing track in the data store.
        /// </summary>
        /// <param name="id">The unique identifier of the track to update.</param>
        /// <param name="track">The updated track details.</param>
        /// <returns>The updated track.</returns>
        Task<Track> UpdateTrackAsync(string id, Track track);

        /// <summary>
        /// Deletes a track by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the track to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        Task<bool> DeleteTrackAsync(string id);

        /// <summary>
        /// Retrieves all tracks from the data store.
        /// </summary>
        /// <returns>A list of all tracks.</returns>
        Task<IEnumerable<Track>> GetAllTracksAsync();
    }
}
