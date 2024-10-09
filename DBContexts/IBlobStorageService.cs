using MoviePlaylist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviePlaylist.DBContexts
{
    public interface IBlobStorageService
    {
        Task ArchivePlaylistAsync(Playlist playlist);
    }
}
