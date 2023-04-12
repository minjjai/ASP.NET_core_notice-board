using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NoticeBoard.Migrations;
using NoticeBoard.Models;

namespace NoticeBoard.Core.Interfaces
{
    public interface INoticeBoardRepository
    {
        //IQueryable<Post> Posts();
        //void AddPosts(Post post);
        DbSet<Post> Posts();
        Task SaveChangesAsync();
        Task<List<SelectListItem>> SelectAsync();
        Task AddAsync(Post post);
        Task AttachAsync(AttachFile attachFile);
        ValueTask<Post?> FindPostAsync(int id);
        Task<List<FixedCategory>> ListAsync();
        Task<Post?> FindAsync(int id);
        Task<List<AttachFile>> FindList(int id);
        Task<Post?> FirstOrDefaultAsyncP(int id);
        Task UpdateAsync(Post post);
        Task<AttachFile?> FirstOrDefaultAsyncA(int id);
        Task<List<string>> SelectByFiledId(int intFileId);
        ValueTask<AttachFile?> FindAsyncA(int intFileId);
        Task RemoveAttachFile(AttachFile deleteAttachFileId);
        Task<List<string>> SelectByPostId(int id);
        Task<List<AttachFile>> FindListAsyncA(int id);
        Task RemoveComments(int id);
        Task RemovePost(Post post);
        Task UpdateAsyncComment(Comment comment);
        DbSet<Comment> Comments();
        ValueTask<Comment?> FIndAsyncComment(int id);
        Task RemoveComment(Comment comment);
        ValueTask<Post?> FindAsyncP(int id);
        Task<bool> PostExistsAsync(int id);
    }
}
