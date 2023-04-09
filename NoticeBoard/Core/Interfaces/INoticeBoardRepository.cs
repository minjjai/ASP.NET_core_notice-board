using System.Collections.Generic;
using System.Threading.Tasks;
using NoticeBoard.Models;

namespace NoticeBoard.Core.Interfaces
{
    public interface INoticeBoardRepository
    {
        Task<List<Post>> ListAsync();
        Task AddAsync(Post post);
    }
}
