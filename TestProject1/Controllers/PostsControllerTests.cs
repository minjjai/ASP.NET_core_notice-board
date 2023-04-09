using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NoticeBoard.Controllers;
using NoticeBoard.Data;
using NoticeBoard.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace NoticeBoard.Tests.Controllers
{
    public class PostsControllerTests
    {


        private PostsController _postsController;
        private NoticeBoardContext _context;
        private IWebHostEnvironment hostEnv;
        public PostsControllerTests()
        {//            var mockContext = new Mock<NoticeBoardContext>(new DbContextOptionsBuilder<NoticeBoardContext>().Options);

            //Dependencies
            _context = A.Fake<NoticeBoardContext>();
            hostEnv = A.Fake<IWebHostEnvironment>();

            //SUT
            _postsController = new PostsController(_context, hostEnv);
        }


        //[Fact]
        //public async void PostsController_Index_ReturnsSuccess()
        //{
        //    //Arrange - What do I need to bring in?
        //    string postCategory = string.Empty;
        //    string searchString = string.Empty;
        //    string sortOrder = string.Empty;
        //    int? page = 1;
        //    int pageSize = 8;
        //    int pageNumber = page.HasValue && page.Value > 0 ? page.Value : 1;

        //    //Act
        //    IQueryable<string> categoryQuery = from m in _context.Posts
        //                                       orderby m.Category
        //                                       select m.Category;
        //    var posts = from m in _context.Posts
        //                select m;

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        posts = posts.Where(s => s.Title!.Contains(searchString));
        //    }
        //    if (!string.IsNullOrEmpty(postCategory))
        //    {
        //        posts = posts.Where(x => x.Category == postCategory);
        //    }

        //    switch (sortOrder)
        //    {
        //        default:
        //            posts = posts.OrderByDescending(s => s.LastUpdated);
        //            break;
        //        case "PastOrder":
        //            posts = posts.OrderBy(s => s.LastUpdated);
        //            break;
        //        case "Views":
        //            posts = posts.OrderByDescending(s => s.Views);
        //            break;
        //    }

        //    var totalPosts = await posts.CountAsync();
        //    var totalPages = (int)Math.Ceiling((decimal)totalPosts / pageSize);
        //    var categories = await _context.FixedCategories.Select(c => new SelectListItem
        //    {
        //        Value = c.Categories,
        //        Text = c.Categories
        //    }).ToListAsync();

        //    var viewModel = new PostsViewModel
        //    {
        //        Posts = posts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
        //        Categories = categories,
        //        TotalPages = totalPages,
        //        CurrentPage = pageNumber,
        //        PageSize = pageSize,
        //        PostCategory = postCategory,
        //        SearchString = searchString,
        //        SortOrder = sortOrder,
        //    };

        //    var result = _postsController.Index(postCategory, searchString, sortOrder, page);
        //    //Assert - Object check actions
        //    result.Should().BeOfType<Task<IActionResult>>();
        //}
        //[Fact]
        //public void PostsController_Detail_ReturnsSuccess()
        //{

        //    var id = 164;
        //    var post = A.Fake<Post>();
        //    A.CallTo(() => _context.Posts.Include(x => x.Comments)
        //            .Include(x => x.AttachFiles)
        //            .FirstOrDefault(m => m.PostId == id)).Returns(post);
        //    //Act
        //    var result = _postsController.Details(id);
        //    //Assert
        //    result.Should().BeOfType<Task<IActionResult>>();
        //}
    }
}
