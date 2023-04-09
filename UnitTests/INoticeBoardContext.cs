using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTests.Mocks.NoticeBoardContext;
using NoticeBoard.Models;

namespace UnitTests
{
    public interface INoticeBoardContext
    {
        Task<List<Post>> ListAsync();
    }
}
