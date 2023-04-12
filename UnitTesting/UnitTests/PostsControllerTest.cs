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
using Microsoft.AspNetCore.Hosting;
using NoticeBoard.Infrastructure;
using Microsoft.Extensions.FileProviders;
using NuGet.Protocol.Core.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using NoticeBoard;
using Microsoft.AspNetCore.Http;
using Microsoft.Build.Framework;
//using NoticeBoard.Core.Model;

namespace UnitTests.Controllers
{
    public class DependencySetupFixture
    {
        public DependencySetupFixture() //의존성 주입 컨테이너를 설정하는 xUnit 테스트 클래스 생성자
        {
            IConfigurationBuilder builder = new ConfigurationBuilder() //IConfigurationBuilder인스턴스를 생성해서 
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", false, true);//appsetting파일을 읽음
            IConfigurationRoot root = builder.Build();
            var services = new ServiceCollection(); //ServiceCollection 인스턴스를 만듦
            services = (ServiceCollection)IocConfig.Configure(services);
            //IocConfig.Configure메서드를 사용해서 ServiceCollection에 의존성 주입 등록

            ServiceProvider = services.BuildServiceProvider();//BuildServiceProvider메서드로 빌드해서 ServiceProvider속성에 할당
        }

        public ServiceProvider ServiceProvider { get; private set; }
    }
    public class PostsControllerTest : IClassFixture<DependencySetupFixture>
    {
        private readonly IAppDbContext context;
        private readonly INoticeBoardRepository _repository;
        public PostsControllerTest(DependencySetupFixture fixture)
        {
            _repository = fixture.ServiceProvider.GetService<INoticeBoardRepository>();
            context = fixture.ServiceProvider.GetService<IAppDbContext>();
            AddTestData(context);
        }

        [Theory]
        [InlineData(null, null, "PastOrder", 1)]
        public async Task Index_ReturnsAViewResult_PostsViewModel(string postCategory, string searchString, string sortOrder, int? page)
        {
            //Arrange
            var controller = new PostsController(_repository);

            //Act 
            var result = await controller.Index(postCategory, searchString, sortOrder, page);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<PostsViewModel>(viewResult.ViewData.Model); //object가 T타입으로 반환 가능한지 검사
            Assert.Equal(postCategory, model.PostCategory);
            Assert.Equal(searchString, model.SearchString);
            Assert.Equal(sortOrder, model.SortOrder);
            Assert.Equal(page, model.CurrentPage);
        }

        [Theory]
        [InlineData(1)]
        public async Task Details_ReturnsAViewResult_Posts(int id)
        {
            //Araange
            var controller = new PostsController(_repository);

            //Act
            var result = await controller.Details(id);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result); // object의 타입이 T 타입이면 T 타입으로 변환
            var model = Assert.IsAssignableFrom<Post>(viewResult.ViewData.Model);
            Assert.Equal(id, model.PostId);
        }

        [Fact]
        public async Task Create_ReturnsAViewResult_Post()
        {
            //Arrange
            var controller = new PostsController(_repository);

            //Act
            var result = await controller.Create();

            //Assert
            var FixedCategories = await _repository.SelectAsync();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CategoryViewModel>(viewResult.ViewData.Model);
            Assert.IsType<CategoryViewModel>(model);
            Assert.Equal(FixedCategories, model.Categories);
        }

        [Theory]
        [InlineData("Nickname", "Test Title", "Test Content", "Test Category", null)]
        public async Task Create_ReturnsRedirect_Post(string Nickname, string Title, string Content, string Category, ICollection<IFormFile>? Files)
        {
            //Arrange
            var controller = new PostsController(_repository);

            //Act
            var post = new Post { Nickname = Nickname, Title = Title, Content = Content, Category = Category };
            var result = await controller.Create(post, null);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_Post()
        {
            //Arrange 
            var controller = new PostsController(_repository);

            //Act
            var result = await controller.Edit(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CategoryViewModel>(viewResult.Model);
            Assert.Equal(1, model.PostId);
        }

        [Fact]
        public async Task GetFiles_ReturnsJsonResult_AttackFile()
        {
            //Arrange
            var controller = new PostsController(_repository);

            //Act
            var result = await controller.GetFiles(1);

            //Assert
            Assert.IsType<JsonResult>(result);
        }

        [Theory]
        [InlineData(1, null)]
        public async Task Edit_ReturnsRedirect_Post(int id, [FromForm] string? FileId = null )
        {
            //Arrange
            ICollection<IFormFile> files = Enumerable.Empty<IFormFile>().ToList();
            var controller = new PostsController(_repository);
            var post = new Post { PostId = 1, Nickname = "Nickname", Title = "Title", Content = "Content", Category = "Category" };

            //Act
            var result = await controller.Edit(id, post, files, FileId);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsOk_Contextt()
        {
            //Arrange 
            var controller = new PostsController (_repository);

            //Act
            var result = await controller.DeleteConfirmed(1);

            //Assert
            Assert.IsType<OkResult>(result);
        }

        [Theory]
        [InlineData("1")]
        public async Task DeleteIds_ReturnsOk_Context(string id)
        {
            //Arrange 
            var controller = new PostsController(_repository);

            //Act
            var result = await controller.DeleteIds(id);

            //Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteC_ReturnsRedirectWithObject_Post()
        {
            //Arrange
            var controller = new PostsController( _repository);
            context.Add(new Comment()
            {
                CommentId = 1,
                Content = "Content",
                LastUpdated = DateTime.Now,
                PostId = 1
            });
            //Act
            var result = await controller.DeleteC(1);

            //Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(redirectResult.RouteValues["id"], 1);
        }

        private static void AddTestData(IAppDbContext context)
        {
            context.Add(new Post()
            {
                PostId = 1,
                Title = "Dummy Post 1",
                Content = "This is a dummy post.",
                LastUpdated = DateTime.Now,
                Views = 0,
                Category = "1",
                Nickname = "1"
            });

            context.Add(new AttachFile()
            {
                FileId = 1,
                FileName = "aaa.jpg",
                FilePath = "c:/Users/Minjae13.kim/FilePath",
                PostId = 1
            });
        }
    }
}
