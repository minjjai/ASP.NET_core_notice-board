using Microsoft.EntityFrameworkCore;
using NoticeBoard.Models;

namespace NoticeBoard.Infrastructure
{
    public interface IAppDbContext 
    {
         DbSet<Post> Posts { get; set; }
         DbSet<Comment> Comments { get; set; }
         DbSet<FixedCategory> FixedCategories { get; set; }
         DbSet<AttachFile> AttachFiles { get; set; }

        void Add(Post post);
        void Add(AttachFile attachFile);
        void Add(Comment comment);
        Task SaveChangesAsync();
    }
}
