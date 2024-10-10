using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviePlaylist.Services
{
    public interface IQueueService
    {
        Task QueueUserPlaylist(UserCurrentPlaylist userCurrentPlaylist);
        Task<UserCurrentPlaylist> ReceiveMessageAsync();
    }
}
