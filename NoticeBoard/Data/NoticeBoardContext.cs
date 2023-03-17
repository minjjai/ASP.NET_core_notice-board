using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NoticeBoard.Models;

namespace NoticeBoard.Data
{
    public class NoticeBoardContext : DbContext
    {
        public DbSet<NoticeBoard.Models.Post> Post { get; set; }
        //public DbSet<NoticeBoard.Models.Comment> Comment { get; set; }
        public DbSet<Post> Posts { get; set; } 
        public DbSet<Comment> Comments { get; set; }
        public NoticeBoardContext(DbContextOptions<NoticeBoardContext> options)
            : base(options)
        {
        }

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
