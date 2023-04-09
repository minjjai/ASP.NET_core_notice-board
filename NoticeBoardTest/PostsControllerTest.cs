using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NoticeBoard.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoticeBoardTest
{
    //    [TestClass]
    //    public class PostsControllerTest
    //    {
    //        [TestMethod]
    //        public void add_new_posts()
    //        {
    //        }
    //    }
    //}
    //public class NoticeBoardContext : DbContext
    //{
    //    public virtual DbSet<Post> Posts { get; set; }
    //    public virtual DbSet<AttachFile> AttachFiles { get; set; }
    //}
    //public interface INoticeBoardContext
    //{
    //    IDbSet<Post> Posts { get; set; }
    //    IDbSet<AttachFile> AttachFiles { get; set; }
    //}

    //public class MockNoticeBoardContext : NoticeBoard.Data.NoticeBoardContext, INoticeBoardContext
    //{
    //    public IDbSet<Post> Posts { get; set; }
    //    public IDbSet<AttachFile> AttachFiles { get; set; }
    //    // ...

    //    public MockNoticeBoardContext()
    //    {
    //        var data = new List<Post>
    //    {
    //        new Post { PostId = 1, Title = "Dummy Post 1", Content = "This is a dummy post.",LastUpdated = DateTime.Now, Views = 0, Category = "1", Nickname = "1" }
    //    }.AsQueryable();

    //        var mockSet = new Mock<IDbSet<Post>>();
    //        mockSet.As<IQueryable<Post>>().Setup(m => m.Provider).Returns(data.Provider);
    //        mockSet.As<IQueryable<Post>>().Setup(m => m.Expression).Returns(data.Expression);
    //        mockSet.As<IQueryable<Post>>().Setup(m => m.ElementType).Returns(data.ElementType);
    //        mockSet.As<IQueryable<Post>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

    //        this.Posts = mockSet.Object;

    //        var SampleData2 = new List<AttachFile>
    //    {
    //        new AttachFile { FileId = 1, FileName = "aaa.jpg", FilePath = "c:/Users/Minjae13.kim/FilePath", PostId = 1}
    //    }.AsQueryable();

    //        var mockSet2 = new Mock<IDbSet<AttachFile>>();
    //        mockSet2.As<IQueryable<AttachFile>>().Setup(m => m.Provider).Returns(SampleData2.Provider);
    //        mockSet2.As<IQueryable<AttachFile>>().Setup(m => m.Expression).Returns(SampleData2.Expression);
    //        mockSet2.As<IQueryable<AttachFile>>().Setup(m => m.ElementType).Returns(SampleData2.ElementType);
    //        mockSet2.As<IQueryable<AttachFile>>().Setup(m => m.GetEnumerator()).Returns(() => SampleData2.GetEnumerator());

    //        this.AttachFiles = mockSet2.Object;
    //    }
    //}
    //public class PostsControllerTest
    //{
    //    private readonly MockNoticeBoardContext _context;
    //    private IWebHostEnvironment hostEnv;
    //    public PostsControllerTest(MockNoticeBoardContext context, IWebHostEnvironment env)
    //    {
    //        _context = context;
    //        hostEnv = env;
    //    }
    //}
}