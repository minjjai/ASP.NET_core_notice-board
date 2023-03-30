using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NoticeBoard.Data;
using NoticeBoard.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Mail;
using Azure.Core;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using NoticeBoard.Migrations;
using System.Text.RegularExpressions;

namespace NoticeBoard.Controllers
{
    //종속성 주입
    public class PostsController : Controller
    {
        private readonly NoticeBoardContext _context;
        private IWebHostEnvironment hostEnv;

        public PostsController(NoticeBoardContext context, IWebHostEnvironment env)
        {
            _context = context;
            hostEnv = env;
        }

        // GET: Posts
        public async Task<IActionResult> Index(string postCategory, string searchString, string sortOrder, int? page)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'NoticeBoard.Post'  is null.");
            }

            int pageSize = 8;
            int pageNumber = page.HasValue && page.Value > 0 ? page.Value : 1;
            //페이지값이 널인경우 1을 반환 1보다 안작아서 페이지값가져옴 페이지가 널이아니면 페이지값반환 1보다 작을시 1을 반환

            // Use LINQ to get list of genres.
            IQueryable<string> categoryQuery = from m in _context.Posts
                                               orderby m.Category
                                               select m.Category;

            var posts = from m in _context.Posts
                        select m;


            if (!string.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(s => s.Title!.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(postCategory))
            {
                posts = posts.Where(x => x.Category == postCategory);
            }

            switch (sortOrder)
            {
                default:
                    posts = posts.OrderByDescending(s => s.LastUpdated);
                    break;
                case "PastOrder":
                    posts = posts.OrderBy(s => s.LastUpdated);
                    break;
                case "Views":
                    posts = posts.OrderByDescending(s => s.Views);
                    break;
            }

            var totalPosts = await posts.CountAsync();
            var totalPages = (int)Math.Ceiling((decimal)totalPosts / pageSize);
            var categories = await _context.FixedCategories.Select(c => new SelectListItem
            {
                Value = c.Categories,
                Text = c.Categories
            }).ToListAsync();

            var viewModel = new PostsViewModel
            {
                Posts = await posts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(),
                Categories = categories,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                PostCategory = postCategory,
                SearchString = searchString,
                SortOrder = sortOrder,
        };

            return View(viewModel);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(x => x.Comments)
                .Include(x => x.AttachFiles)
                .FirstOrDefaultAsync(m => m.PostId == id);

            post.Views++;
            //_context.Update(post);
            await _context.SaveChangesAsync();

            if (post == null)
            {
                return NotFound();
            }

            ViewData["Views"] = post.Views;
            return View(post);
        }

        // GET: Posts/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _context.FixedCategories.Select(c => new SelectListItem
            {
                Value = c.Categories,
                Text = c.Categories
            }).ToListAsync();

            var model = new CategoryViewModel
            {
                Categories = categories
            };

            return View(model);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Post post, [FromForm] ICollection<IFormFile>? Files = null)
        {
            DateTime datetime = DateTime.Now;
            post.LastUpdated = datetime;
            _context.Add(post);
            await _context.SaveChangesAsync();

            if (Request.Form.Files.Count > 0)
            {
                var uploadPath = Path.Combine(hostEnv.WebRootPath, "Files");
                var files = Request.Form.Files;
                for (var i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(uploadPath, fileName).Replace("\\", "/");
                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(fs); //업로드된 파일 타겟의 내용을 비동기적으로 복사
                    }


                    //string combinedFilePaths = string.Join(",", filePaths);

                    var attachFile = new AttachFile
                    {
                        FileName = file.FileName,
                        FilePath = filePath,
                        FileData = System.IO.File.ReadAllBytes(filePath),
                        PostId = post.PostId
                    };
                    _context.Attach(attachFile); //이미 데이터베이스에 있는 엔터티를 수정하려는 경우 사용
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var uploadPath = Path.Combine(hostEnv.WebRootPath, "Files");
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { url = $"/files/{fileName}" });
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (await _context.AttachFiles.FirstOrDefaultAsync(p => p.PostId == id) != null)
            {
                var categories = await _context.FixedCategories.ToListAsync();
                //var attachFiles = await _context.AttachFiles.ToListAsync();
                var viewModel = new CategoryViewModel
                {
                    PostId = post.PostId,
                    Category = post.Category,
                    Nickname = post.Nickname,
                    Title = post.Title,
                    Content = post.Content,
                    Categories = categories.Select(c => new SelectListItem
                    {
                        Text = c.Categories,
                        Value = c.Categories,
                        Selected = (post.Category == c.Categories)
                    }),
                    //FileNames = attachFiles.Select(c => new SelectListItem
                    //{
                    //    Text = c.FileName,
                    //    Value = c.FileName,
                    //    Selected = (post.PostId == c.PostId)
                    //}),
                };

                return View(viewModel);
            }
            else
            {

                var categories = await _context.FixedCategories.ToListAsync();
                var viewModel = new CategoryViewModel
                {
                    PostId = post.PostId,
                    Nickname = post.Nickname,
                    Title = post.Title,
                    Content = post.Content,
                    Categories = categories.Select(c => new SelectListItem
                    {
                        Text = c.Categories,
                        Value = c.Categories,
                        Selected = (post.Category == c.Categories)
                    })
                };

                return View(viewModel);
            }


        }


        [HttpGet]
        public IActionResult GetFiles(int id)
        {
            var files = _context.AttachFiles.Where(p => p.PostId == id).ToList();
            var result = files.Select(p => new
            {
                FileId = p.FileId,
                FileName = p.FileName,
                FileSize = p.FileData.Length
            });
            Console.WriteLine(result);
            return Json(result);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Post post, [FromForm] string? FileId = null, ICollection<IFormFile>? Files = null)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var Post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == id);
                    DateTime datetime = DateTime.Now;
                    Post.Nickname = post.Nickname;
                    Post.Title = post.Title;
                    Post.Content = post.Content;
                    Post.LastUpdated = datetime;
                    Post.Category = post.Category;
                    _context.Update(Post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
            }
            if (Request.Form.Files.Count != 0)
            {
                var uploadPath = Path.Combine(hostEnv.WebRootPath, "Files");
                var files = Request.Form.Files;
                try
                {
                    for (var i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        string filePath = Path.Combine(uploadPath, file.FileName).Replace("\\", "/");
                        using (FileStream fs = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(fs);
                        }
                        // Attachfile 모델에 파일 데이터 저장
                        var attachFile = new AttachFile
                        {
                            FileName = file.FileName,
                            FilePath = filePath,
                            FileData = System.IO.File.ReadAllBytes(filePath),
                            PostId = post.PostId
                        };

                        _context.Attach(attachFile);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    return StatusCode(500);
                }
            }
            if (FileId != null)
            {
                string[] fileIds = FileId.Split(',');
                var array = fileIds.Skip(1).ToArray();
                foreach (var deleteFileId in array)
                {
                    var intFileId = int.Parse(deleteFileId);
                    var filePaths = await _context.AttachFiles
                        .Where(p => p.FileId == intFileId)
                        .Select(p => p.FilePath)
                        .ToListAsync();
                    foreach (var filePath in filePaths)
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    var deleteAttachFileId = await _context.AttachFiles.FindAsync(intFileId);
                    if (deleteAttachFileId != null)
                    {
                        _context.Remove(deleteAttachFileId);
                    }
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Details", new { id = id });
        }

        // GET: Posts/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Posts == null)
        //    {
        //        return NotFound();
        //    }

        //    var post = await _context.Posts
        //        .FirstOrDefaultAsync(m => m.PostId == id);
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(post);
        //}

        // POST: Posts/Delete/5
        [HttpDelete]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'NoticeBoardContext.Post'  is null.");
            }
            var filePaths = await _context.AttachFiles
                .Where(p => p.PostId == id)
                .Select(p => p.FilePath)
                .ToListAsync();

            foreach (var filePath in filePaths)
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            var attachfiles = await _context.AttachFiles
                .Where(p => p.PostId == id)
                .ToListAsync();

            foreach (var attachfile in attachfiles)
            {
                _context.AttachFiles.Remove(attachfile);
            }

            var comments = _context.Comments.Where(c => c.PostId == id);
            _context.Comments.RemoveRange(comments);

            var post = await _context.Posts.FindAsync(id);

            var content = post.Content;
            var regex = new Regex("<img[^>]+src\\s*=\\s*['\"]([^'\"]+)['\"][^>]*>");
            var match = regex.Match(content);
            var rootPath = Path.Combine(hostEnv.WebRootPath, "Files");
            //foreach (Match match in matches) //져러장 업로드는 돈내야 한단다
            //{
            var imageUrl = match.Groups[1].Value;
            var imageName = Path.GetFileName(imageUrl);
            var imagePath = Path.Combine(rootPath, imageName);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            //}

            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteIds(string id)
        {
            string[] splitId = id.Split(",");

            if (_context.Posts == null)
            {
                return BadRequest();
            }

            bool Remove = false;

            for (int i = 0; i < splitId.Length; i++)
            {
                var intId = int.Parse(splitId[i]);
                var filePaths = await _context.AttachFiles
                                .Where(p => p.PostId == intId)
                                .Select(p => p.FilePath)
                                .ToListAsync();

                foreach (var filePath in filePaths)
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                var attachfiles = await _context.AttachFiles
                    .Where(p => p.PostId == intId)
                    .ToListAsync();

                foreach (var attachfile in attachfiles)
                {
                    _context.AttachFiles.Remove(attachfile);
                }

                var comments = _context.Comments.Where(c => c.PostId == intId);
                _context.Comments.RemoveRange(comments);

                var post = await _context.Posts.FindAsync(intId);

                var content = post.Content;
                var regex = new Regex("<img[^>]+src\\s*=\\s*['\"]([^'\"]+)['\"][^>]*>");
                var match = regex.Match(content);
                var rootPath = Path.Combine(hostEnv.WebRootPath, "Files");
                //foreach (Match match in matches)
                //{
                var imageUrl = match.Groups[1].Value;
                var imageName = Path.GetFileName(imageUrl);
                var imagePath = Path.Combine(rootPath, imageName);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                //}
                if (post != null)
                {
                    _context.Posts.Remove(post);
                    Remove = true;
                }
            }

            if (Remove)
                await _context.SaveChangesAsync();

            return Ok();
        }
        // POST: Posts/Detail/:postId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, [Bind("CommentId,PostId,Content")] Comment comment)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            DateTime datetime = DateTime.Now;
            comment.LastUpdated = datetime;
            //comment.PostId = id;
            _context.Update(comment);

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = id });
            //}
        }

        // POST: Posts/DeleteC/5
        [HttpPost, ActionName("DeleteC")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteC(int id)
        {
            if (_context.Comments == null)
            {
                return Problem("Entity set 'NoticeBoardContext.Comments'  is null.");
            }
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = comment.PostId });
        }

        private bool PostExists(int id)
        {
            return (_context.Posts?.Any(e => e.PostId == id)).GetValueOrDefault();
        }
    }
}
