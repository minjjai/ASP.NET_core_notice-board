//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using noticeboard.models;
//using NoticeBoard.Core.Interfaces;
//using NoticeBoard.Models;

//namespace NoticeBoard.Core.Model
//{
//    public interface INoticeBoardContext
//    {
//        Task<List<Post>> ListAsync();
//    }
//    public class NoticeBoardContext : DbContext, INoticeBoardContext
//    {
//        //public DbSet<NoticeBoard.Models.Comment> Comment { get; set; }
//        public virtual DbSet<Post> Posts { get; set; }
//        public DbSet<Comment> Comments { get; set; }
//        public DbSet<FixedCategory> FixedCategories { get; set; }
//        public virtual DbSet<AttachFile> AttachFiles { get; set; }

//        public NoticeBoardContext() { }
//        public NoticeBoardContext(DbContextOptions<NoticeBoardContext> options)
//            : base(options)
//        {
//        }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=NoticeBoardContext;User ID=sa;Password=123qwe!@#QWE;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;");
//        }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<Post>()
//                .Property(p => p.Views)
//                .HasDefaultValue(0);
//            modelBuilder.Entity<FixedCategory>();
//        }

//        public Task<List<Post>> ListAsync()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
