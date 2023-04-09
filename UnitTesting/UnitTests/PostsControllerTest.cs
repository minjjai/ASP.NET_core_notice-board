using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoticeBoard.Controllers;
using Moq;
using NoticeBoard.Models;
using Xunit;
using NoticeBoard.Core.Interfaces;
using NoticeBoard.Core.Model;

namespace UnitTests.Controllers
{

    //public static class SampleData
    //{
    //    public static List<Post> GetPosts()
    //    {
    //        return new List<Post>
    //        {
    //            new Post
    //            {
    //                PostId = 1,
    //                Title = "Dummy Post 1",
    //                Content = "This is a dummy post.",
    //                LastUpdated = DateTime.Now,
    //                Views = 0, Category = "1",
    //                Nickname = "1"
    //            }
    //        };
    //    }
    //    public static List<AttachFile> GetAttachFiles()
    //    {
    //        return new List<AttachFile>
    //        {
    //            new AttachFile {
    //                FileId = 1,
    //                FileName = "aaa.jpg",
    //                FilePath = "c:/Users/Minjae13.kim/FilePath",
    //                PostId = 1
    //            }
    //        };
    //    }
    //}

    //public static class TestStartup
    //{
    //    public static void ConfigureServices(IServiceCollection services)
    //    {
    //        var options = new DbContextOptionsBuilder<NoticeBoardContext>()
    //          .UseInMemoryDatabase(databaseName: "MockDb")
    //          .Options;
    //        var context = new MockNoticeBoardContext(options);
    //        var mockSet = new Mock<IDbSet<Post>>();
    //        mockSet.As<IQueryable<Post>>().Setup(m => m.Provider).Returns(SampleData.GetPosts().AsQueryable().Provider);
    //        mockSet.As<IQueryable<Post>>().Setup(m => m.Expression).Returns(SampleData.GetPosts().AsQueryable().Expression);
    //        mockSet.As<IQueryable<Post>>().Setup(m => m.ElementType).Returns(SampleData.GetPosts().AsQueryable().ElementType);
    //        mockSet.As<IQueryable<Post>>().Setup(m => m.GetEnumerator()).Returns(() => SampleData.GetPosts().AsQueryable().GetEnumerator());

    //        context.Posts = mockSet.Object;

    //        var mockSet2 = new Mock<IDbSet<AttachFile>>();
    //        mockSet2.As<IQueryable<AttachFile>>().Setup(m => m.Provider).Returns(SampleData.GetAttachFiles().AsQueryable().Provider);
    //        mockSet2.As<IQueryable<AttachFile>>().Setup(m => m.Expression).Returns(SampleData.GetAttachFiles().AsQueryable().Expression);
    //        mockSet2.As<IQueryable<AttachFile>>().Setup(m => m.ElementType).Returns(SampleData.GetAttachFiles().AsQueryable().ElementType);
    //        mockSet2.As<IQueryable<AttachFile>>().Setup(m => m.GetEnumerator()).Returns(() => SampleData.GetAttachFiles().GetEnumerator());

    //        context.AttachFiles = mockSet2.Object;
    //        services.AddScoped<INoticeBoardContext, NoticeBoardContext>(_ => context);
    //    }
    //}
    public class PostsControllerTest
    {
        //private readonly NoticeBoardContext _context;
        //private IWebHostEnvironment hostEnv;
        //public PostsControllerTest(NoticeBoardContext context)
        //{
        //    _context = context;
        //    hostEnv = new Mock<IWebHostEnvironment>().Object;
        //}
        ////index에서 리턴되는 뷰모델의 값들이 들어오는 값과 같은지 확인
        //[Theory]
        //[InlineData(null, null, "PastOrder", 1)]
        //public async Task Index_ReturnsAViewResult_PostsViewModel(string postCategory, string searchString, string sortOrder, int? page)
        //{
        //    //Arrange
        //    var options = new DbContextOptionsBuilder<NoticeBoardContext>().Options;
        //    using var context = new MockNoticeBoardContext(options);

        //    var mockEnvironment = new Mock<IWebHostEnvironment>();

        //    var controller = new PostsController(context, mockEnvironment.Object);

        //    //Act 
        //    var result = await controller.Index(postCategory, searchString, sortOrder, page);

        //    //Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);

        //    var model = Assert.IsAssignableFrom<PostsViewModel>(viewResult.ViewData.Model);
        //    Assert.Equal(postCategory, model.PostCategory);
        //    Assert.Equal(searchString, model.SearchString);
        //    Assert.Equal(sortOrder, model.SortOrder);
        //    Assert.Equal(page, model.CurrentPage);
        //}

        ////들어오는 id값과 반환되는 게시글의 id값과 같은지 확인
        //[Theory]
        //[InlineData(1)]
        //public async Task Details_RturnsAViewResult_Posts(int id)
        //{
        //    //Araange
        //    var options = new DbContextOptionsBuilder<NoticeBoardContext>().Options;
        //    using var context = new MockNoticeBoardContext(options);
        //    var mockEnvironment = new Mock<IWebHostEnvironment>();

        //    var controller = new PostsController(context, mockEnvironment.Object);

        //    //Act
        //    var result = await controller.Details(id);

        //    //Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsAssignableFrom<Post>(viewResult.ViewData.Model);
        //    Assert.Equal(id, model.PostId);

        //}

        ////카테고리뷰모델형식이 반환되는지 확인
        //[Fact]
        //public async Task Create_ReturnsAViewResult()
        //{
        //    //Arrange
        //    var options = new DbContextOptionsBuilder<NoticeBoardContext>().Options;
        //    using var context = new MockNoticeBoardContext(options);
        //    var mockEnvironment = new Mock<IWebHostEnvironment>();
        //    var controller = new PostsController(context, mockEnvironment.Object);

        //    var categories = await context.FixedCategories.Select(c => new SelectListItem
        //    {
        //        Value = c.Categories,
        //        Text = c.Categories
        //    }).ToListAsync();

        //    //Act
        //    var result = await controller.Create();

        //    //Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);

        //    var model = Assert.IsAssignableFrom<CategoryViewModel>(viewResult.ViewData.Model);
        //    Assert.IsType<CategoryViewModel>(model);
        //}

        ////값이 들어오면 index페이지로 리다이렉트 되는지...
        //[Theory]
        //[InlineData("Nickname", "Test Title", "Test Content", "Test Category", null)]
        //public async Task Create_Post(string Nickname, string Title, string Content, string Category, ICollection<IFormFile>? Files)
        //{
        //    //Arrange
        //    var options = new DbContextOptionsBuilder<NoticeBoardContext>().Options;
        //    using var context = new MockNoticeBoardContext(options);
        //    var mockEnvironment = new Mock<IWebHostEnvironment>();
        //    var controller = new PostsController(context, mockEnvironment.Object);

        //    //Act
        //    var post = new Post { Nickname = Nickname, Title = Title, Content = Content, Category = Category };

        //    ICollection<IFormFile> files = new List<IFormFile>
        //    {
        //        new FormFile(Stream.Null, 0, 0, "file1", "file1.txt"),
        //        new FormFile(Stream.Null, 0, 0, "file2", "file2.jpg"),
        //        new FormFile(Stream.Null, 0, 0, "file3", "file3.pdf")
        //    };
        //    var result = await controller.Create(post, null);

        //    //Assert
        //    var RedirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        //}

        //id값으로 뷰리절트 반환
        [Fact]
        public async Task Edit_ReturnsViewResult_Post()
        {
            //Arrange
            var mockContext = new Mock<INoticeBoardContext>();
            mockContext.Setup(mockContext => mockContext.ListAsync())
                .ReturnsAsync(GetTestPosts());
            mockContext.Setup(mockContext => mockContext.ListAsync())
                .ReturnsAsync(GetTestAttachFiles());
            var env = new Mock<IWebHostEnvironment>();

            var controller = new PostsController(mockContext.Object, env.Object);

            //Act
            var result = await controller.Edit(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CategoryViewModel>(viewResult.Model);
            Assert.Equal(1, model.PostId);
        }
        public List<Post> GetTestPosts()
        {
            var posts = new List<Post>();
            posts.Add(new Post()
            {
                PostId = 1,
                Title = "Dummy Post 1",
                Content = "This is a dummy post.",
                LastUpdated = DateTime.Now,
                Views = 0,
                Category = "1",
                Nickname = "1"
            });
            return posts;
        }

        public List<AttachFile> GetTestAttachFiles()
        {
            var attachFiles = new List<AttachFile>();
            attachFiles.Add(new AttachFile
            {
                FileId = 1,
                FileName = "aaa.jpg",
                FilePath = "c:/Users/Minjae13.kim/FilePath",
                PostId = 1
            });
            return attachFiles;
        }
    }
}
