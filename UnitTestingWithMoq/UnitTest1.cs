using Microsoft.AspNetCore.Mvc;
using NoticeBoard.Controllers;
using NoticeBoard.Models;
using NoticeBoard.Core.Interfaces;
using NoticeBoard.Infrastructure;
using NoticeBoard;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Linq.Expressions;
using NuGet.Protocol.Core.Types;
using Microsoft.EntityFrameworkCore.Query;
using static UnitTestingWithMoq.PostsControllerTest;
using TestingDemo;
using Microsoft.Extensions.Hosting;
using System.Xml.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NoticeBoard.Migrations;
using Castle.Core.Resource;

namespace UnitTestingWithMoq
{
    public class PostsControllerTest
    {
        private readonly Mock<IAppDbContext> context;
        private readonly Mock<INoticeBoardRepository> _repository;
        public PostsControllerTest()
        {
            context = new Mock<IAppDbContext>();
            context.Setup(c => c.Posts).Returns(GetTestDetailPost());
            _repository = new Mock<INoticeBoardRepository>();
        }

        [Theory]
        [InlineData(null, null, "PastOrder", 1)]
        public async void Index_ReturnsAViewResult_PostsViewModel(string postCategory, string searchString, string sortOrder, int? page)
        {
            //Arrange
            _repository.Setup(c => c.Posts()).Returns(GetTestIndexPosts());
            _repository.Setup(c => c.SelectCountAsync()).Returns(GetTestIndexPosts().Count());
            _repository.Setup(c => c.SelectAsync()).Returns(GetTestCategories());
            var controller = new PostsController(_repository.Object);

            //Act
            var result = await controller.Index(postCategory, searchString, sortOrder, page);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<PostsViewModel>(viewResult.ViewData.Model);
        }

        [Theory]
        [InlineData(1)]
        public async Task Details_ReturnsAViewResult_Posts(int id)
        {
            //Arrange
            _repository.Setup(c => c.FindAsync(id)).Returns(GetDetail(id));
            var controller = new PostsController(_repository.Object);

            //Act
            var result = await controller.Details(id);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Post>(viewResult.ViewData.Model);
            Assert.Equal(id, model.PostId);
        }

        public DbSet<Post> GetTestIndexPosts()
        {
            var posts = new List<Post>
            {
            new Post()
            {
                PostId = 1,
                Title = "Dummy Post 1",
                Content = "This is a dummy post.",
                LastUpdated = DateTime.Now,
                Views = 0,
                Category = "1",
                Nickname = "1"
            }
            };
            var mockSet = new Mock<DbSet<Post>>();

            mockSet.As<IAsyncEnumerable<Post>>().Setup(m => m.GetAsyncEnumerator(CancellationToken.None)).Returns(new TestAsyncEnumerator<Post>(posts));

            mockSet.As<IQueryable<Post>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Post>(posts.AsQueryable().Provider));
            mockSet.As<IQueryable<Post>>().Setup(m => m.Expression).Returns(posts.AsQueryable().Expression);
            mockSet.As<IQueryable<Post>>().Setup(m => m.ElementType).Returns(posts.AsQueryable().ElementType);
            mockSet.As<IQueryable<Post>>().Setup(m => m.GetEnumerator()).Returns(posts.GetEnumerator());

            var result = mockSet.Object;
            return result;
        }

        private async Task<List<SelectListItem>> GetTestCategories()
        {
            var categories = new List<FixedCategory>
            {
            new FixedCategory()
            {
                Id = 1,
                Categories = "OOTD",
            }
            };
            var mockSet = new Mock<DbSet<FixedCategory>>();
            mockSet.As<IAsyncEnumerable<FixedCategory>>().Setup(x => x.GetAsyncEnumerator(default)).Returns(new TestAsyncEnumerator<FixedCategory>(categories.GetEnumerator()));
            mockSet.As<IQueryable<FixedCategory>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<FixedCategory>(categories.AsQueryable().Provider));
            mockSet.As<IQueryable<FixedCategory>>().Setup(m => m.Expression).Returns(categories.AsQueryable().Expression);
            mockSet.As<IQueryable<FixedCategory>>().Setup(m => m.ElementType).Returns(categories.AsQueryable().ElementType);
            mockSet.As<IQueryable<FixedCategory>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());

            //context.Setup(c => c.FixedCategories).Returns(mockSet.Object);
            var result = await mockSet.Object
                .Select(c => new SelectListItem
                {
                    Value = c.Categories,
                    Text = c.Categories
                }).ToListAsync();
            Console.WriteLine(result);
            return result;
        }

        private Task<Post?> GetDetail(int id)
        {
            Console.WriteLine(context.Object);
            return context.Object.Posts
                .Include(c => c.Comments)
                .Include(c => c.AttachFiles)
                .FirstOrDefaultAsync(c => c.PostId == id);
        }
        public DbSet<Post> GetTestDetailPost()
        {
            var comment = new Comment()
            {
                CommentId = 1,
                Content = "This is a dummy comment.",
                LastUpdated = DateTime.Now,
                PostId = 1,
            };
            //post.Comments.Add(comment);
            var attachFile = new AttachFile()
            {
                FileId = 1,
                FileName = "aaa.jpg",
                FilePath = "c:/Users/Minjae13.kim/FilePath",
                PostId = 1
            };
            //post.AttachFiles.Add(attachFile);

            var comments = new List<Comment>
            {
                comment
            };
            var mockCommentSet = new Mock<DbSet<Comment>>();
            mockCommentSet.As<IQueryable<Comment>>().Setup(m => m.Provider).Returns(comments.AsQueryable().Provider);
            mockCommentSet.As<IQueryable<Comment>>().Setup(m => m.Expression).Returns(comments.AsQueryable().Expression);
            mockCommentSet.As<IQueryable<Comment>>().Setup(m => m.ElementType).Returns(comments.AsQueryable().ElementType);
            mockCommentSet.As<IQueryable<Comment>>().Setup(m => m.GetEnumerator()).Returns(comments.AsQueryable().GetEnumerator());

            var attachFiles = new List<AttachFile>
            { 
                attachFile
            };
            var mockAttachFileSet = new Mock<DbSet<AttachFile>>();
            mockAttachFileSet.As<IQueryable<AttachFile>>().Setup(m => m.Provider).Returns(attachFiles.AsQueryable().Provider);
            mockAttachFileSet.As<IQueryable<AttachFile>>().Setup(m => m.Expression).Returns(attachFiles.AsQueryable().Expression);
            mockAttachFileSet.As<IQueryable<AttachFile>>().Setup(m => m.ElementType).Returns(attachFiles.AsQueryable().ElementType);
            mockAttachFileSet.As<IQueryable<AttachFile>>().Setup(m => m.GetEnumerator()).Returns(attachFiles.AsQueryable().GetEnumerator());
            
            var post = new Post()
            {
                PostId = 1,
                Title = "Dummy Post 1",
                Content = "This is a dummy post.",
                LastUpdated = DateTime.Now,
                Views = 0,
                Category = "1",
                Nickname = "1",
                Comments = comments,
                AttachFiles = attachFiles
            };

            var posts = new List<Post>
            {
                post
            };

            var mockSet = new Mock<DbSet<Post>>();
            mockSet.As<IAsyncEnumerable<Post>>().Setup(m => m.GetAsyncEnumerator(CancellationToken.None)).Returns(new TestAsyncEnumerator<Post>(posts));
            mockSet.As<IQueryable<Post>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Post>(posts.AsQueryable().Provider));
            mockSet.As<IQueryable<Post>>().Setup(m => m.Expression).Returns(posts.AsQueryable().Expression);
            mockSet.As<IQueryable<Post>>().Setup(m => m.ElementType).Returns(posts.AsQueryable().ElementType);
            mockSet.As<IQueryable<Post>>().Setup(m => m.GetEnumerator()).Returns(posts.GetEnumerator());
            //mockSet.Setup(m => m.Include(p => p.Comments)).Returns(mockCommentSet.Object);
            //mockSet.Setup(m => m.Include(p => p.AttachFiles)).Returns(mockAttachFileSet.Object);

            context.SetupGet(c => c.Comments).Returns(mockCommentSet.Object);
            context.SetupGet(c => c.AttachFiles).Returns(mockAttachFileSet.Object);
            context.SetupGet(c => c.Posts).Returns(mockSet.Object);
            context.Object.Posts.Include(c => c.Comments);
            context.Object.Posts.Include(c =>c.AttachFiles);
            var result = context.Object.Posts;
            return result;
        }
        //public DbSet<Comment> GetTestComments()
        //{
        //    var comments = new List<Comment>
        //    {
        //    new Comment()
        //    {
        //        CommentId = 1,
        //        Content = "This is a dummy comment.",
        //        LastUpdated = DateTime.Now,
        //        PostId = 1,
        //    }
        //    };
        //    var mockSet = new Mock<DbSet<Comment>>();
        //    mockSet.As<IAsyncEnumerable<Comment>>().Setup(x => x.GetAsyncEnumerator(default)).Returns(new TestAsyncEnumerator<Comment>(comments.GetEnumerator()));
        //    mockSet.As<IQueryable<Comment>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Comment>(comments.AsQueryable().Provider));
        //    mockSet.As<IQueryable<Comment>>().Setup(m => m.Expression).Returns(comments.AsQueryable().Expression);
        //    mockSet.As<IQueryable<Comment>>().Setup(m => m.ElementType).Returns(comments.AsQueryable().ElementType);
        //    mockSet.As<IQueryable<Comment>>().Setup(m => m.GetEnumerator()).Returns(comments.GetEnumerator());

        //    var result = mockSet.Object;
        //    return result;
        //}
    }
}