using Microsoft.EntityFrameworkCore;
using noticeboard.models;
using NoticeBoard.Models;

namespace NoticeBoard.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) :
    base(dbContextOptions)
        {
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<FixedCategory> FixedCategories { get; set; }
        public virtual DbSet<AttachFile> AttachFiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=NoticeBoardContext;User ID=sa;Password=123qwe!@#QWE;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .Property(p => p.Views)
                .HasDefaultValue(0);
        }
    }
}
