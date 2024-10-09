using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviePlaylist.Repositories
{
    public interface IUserHistoryRepository
    {
        Task SaveUserPlaylistAsync(UserCurrentPlaylist userPlaylist);
    }
}
