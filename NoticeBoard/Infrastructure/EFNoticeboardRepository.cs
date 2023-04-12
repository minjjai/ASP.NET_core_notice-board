using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NoticeBoard.Core.Interfaces;
using NoticeBoard.Models;
using NuGet.Protocol.Core.Types;

namespace NoticeBoard.Infrastructure
{
    public class EFNoticeboardRepository : INoticeBoardRepository
    {
        private readonly IAppDbContext _context;
        public EFNoticeboardRepository(IAppDbContext dbContext)
        {
            _context = dbContext;
        }

        //List<Post> data = new List<Post>
        //{
        //    new Post()
        //    {
        //        PostId = 1,
        //        Title = "Dummy Post 1",
        //        Content = "This is a dummy post.",
        //        LastUpdated = DateTime.Now,
        //        Views = 0,
        //        Category = "1",
        //        Nickname = "1"
        //    }
        //};
        //public IQueryable<Post> Posts()
        //{
        //    return this.data.AsQueryable();
        //}
        //public void AddPosts(Post post)
        //{
        //    this.data.Add(post);
        //}
        public DbSet<Post> Posts()
        {
            return _context.Posts;
        }

        public Task SaveChangesAsync()
        {
             return _context.SaveChangesAsync();
        }
        public Task<List<SelectListItem>> SelectAsync()
        {
            return _context.FixedCategories.Select(c => new SelectListItem
            {
                Value = c.Categories,
                Text = c.Categories
            }).ToListAsync();

        }
        public Task<Post?> FindAsync(int id)
        {
            return _context.Posts
                .Include(x => x.Comments)
                .Include(x => x.AttachFiles)
                .FirstOrDefaultAsync(m => m.PostId == id);
        }
        public Task AddAsync(Post post)
        {
            _context.Posts.Add(post);
            return _context.SaveChangesAsync();
        }
        public Task AttachAsync(AttachFile attachFile)
        {
             _context.AttachFiles.Attach(attachFile);
            return _context.SaveChangesAsync();
        }

        public ValueTask<Post?> FindPostAsync(int id)
        {
            return _context.Posts.FindAsync(id);
        }
        public Task<List<FixedCategory>> ListAsync()
        {
            return _context.FixedCategories.ToListAsync();
        }

        public Task <List<AttachFile>> FindList(int id)
        {
            return _context.AttachFiles.Where(p => p.PostId == id).ToListAsync();
        }

        public Task<Post?> FirstOrDefaultAsyncP(int id)
        {
            return _context.Posts.FirstOrDefaultAsync(p => p.PostId == id);
        }

        public Task UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            return _context.SaveChangesAsync();
        }

        public Task<AttachFile?> FirstOrDefaultAsyncA(int id)
        {
            return _context.AttachFiles.FirstOrDefaultAsync(p => p.PostId == id);
        }
        public Task<List<string>> SelectByFiledId(int intFileId)
        {
            return _context.AttachFiles
                .Where(p => p.FileId == intFileId)
                .Select(propa => propa.FilePath)
                .ToListAsync();
        }

        public ValueTask<AttachFile?> FindAsyncA(int intFileId)
        {
            return _context.AttachFiles.FindAsync(intFileId);
        }

        public Task RemoveAttachFile(AttachFile attachfile)
        {
            _context.AttachFiles.Remove(attachfile);
            return _context.SaveChangesAsync(); 
        }

        public Task<List<string>> SelectByPostId(int id)
        {
            return _context.AttachFiles.Where(p => p.PostId == id)
                .Select(p => p.FilePath) .ToListAsync();
        }
        public Task<List<AttachFile>> FindListAsyncA(int id)
        {
            return _context.AttachFiles
                .Where(p => p.PostId == id)
                .ToListAsync();
        }

        public async Task RemoveComments(int id)
        {
            var comments = _context.Comments.Where(c => c.PostId == id);
                 _context.Comments.RemoveRange(comments);
             await _context.SaveChangesAsync();
        }

        public ValueTask<Post?> FindAsyncP(int id)
        {
            return _context.Posts.FindAsync(id);
        }

        public async Task RemovePost(Post post)
        {
             _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsyncComment(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public DbSet<Comment> Comments()
        {
            return _context.Comments;
        }

        public ValueTask<Comment?> FIndAsyncComment(int id)
        {
            return _context.Comments.FindAsync(id);
        }
        
        public async Task RemoveComment(Comment comment)
        {
             _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> PostExistsAsync(int id)
        {
            return await Task.FromResult(_context.Posts?.Any(e => e.PostId == id) ?? false);
        }
    }
}
