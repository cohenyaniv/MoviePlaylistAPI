using Models;
using MoviePlaylist.Models;
using MoviePlaylist.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviePlaylist.DBContexts
{
    public interface IBlobStorageService
    {
        Task ArchivePlaylistAsync(UserCurrentPlaylist playlist);
    }
}
