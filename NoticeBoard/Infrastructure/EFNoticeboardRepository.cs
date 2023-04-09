using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using noticeboard.models;
using NoticeBoard.Core.Interfaces;
using NoticeBoard.Models;
using NuGet.Protocol.Core.Types;

namespace NoticeBoard.Infrastructure
{
    public class EFNoticeboardRepository : INoticeBoardRepository
    {
        private readonly AppDbContext _context;
        public EFNoticeboardRepository(AppDbContext dbContext)
        {
            _context = dbContext;
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
        public Task<Post> FindAsync(int id)
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
        public Task<EntityEntry<AttachFile>> Attach(AttachFile attachFile)
        {
            return Task.FromResult(_context.AttachFiles.Attach(attachFile));
        }
        public Task<List<FixedCategory>> ListAsync()
        {
            return _context.FixedCategories.ToListAsync();
        }

        public Task <List<AttachFile>> FindList(int id)
        {
            return _context.AttachFiles.Where(p => p.PostId == id).ToListAsync();
        }

        public Task UpdateAsync(Post post)
        {
            throw new NotImplementedException();
        }

        public Task<Post> FirstOrDefaultAsyncP(int id)
        {
            return _context.Posts.FirstOrDefaultAsync(p => p.PostId == id);
        }

        public Task<AttachFile> FirstOrDefaultAsync(int id)
        {
            return _context.AttachFiles.FirstOrDefaultAsync(p => p.PostId == id);
        }
        public Task<List<AttachFile>> SelectList()
        {
            return _context.Attach
        }

        public Task<FixedCategory> ToListAsync()
        {
            throw new NotImplementedException();
        }
    }
}
