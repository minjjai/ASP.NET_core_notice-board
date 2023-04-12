using Microsoft.EntityFrameworkCore;
using NoticeBoard.Models;

namespace NoticeBoard.Infrastructure
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
        //public DbSet<Post> Post { get; set; }
        public DbSet<Post> Posts { get; set; }
        //public DbSet<Comment> Comment { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<FixedCategory> FixedCategories { get; set; }
        public virtual DbSet<AttachFile> AttachFiles { get; set; }

        public void Add(Post post)
        {
            Posts.Add(post);
            this.SaveChanges();
        }

        public void Add(AttachFile attachFile)
        {
            AttachFiles.Add(attachFile);
            this.SaveChanges();
        }
        public void Add(Comment comment)
        {
            Comments.Add(comment);
            this.SaveChanges();
        }
        public Task SaveChangesAsync() => base.SaveChangesAsync();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>().ToTable("Post");

            modelBuilder.Entity<Post>()
                .Property(p => p.Views)
                .HasDefaultValue(0);
        }
    }
}
