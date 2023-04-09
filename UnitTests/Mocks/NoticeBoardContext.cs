using Microsoft.EntityFrameworkCore;
using noticeboard.models;
using NoticeBoard.Data;
using NoticeBoard.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTests.Controllers;

namespace UnitTests.Mocks.NoticeBoardContext
{
    public class NoticeBoardContext : NoticeBoard.Data.NoticeBoardContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<FixedCategory> FixedCategories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AttachFile> AttachFiles { get; set; }
    }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<Post>()
    //    .Property(p => p.Views)
    //    .HasDefaultValue(0);
    //    modelBuilder.Entity<FixedCategory>();



    //}

}
