using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NoticeBoard.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Mail;
using Azure.Core;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using NoticeBoard.Migrations;
using System.Text.RegularExpressions;
using NoticeBoard.Core.Interfaces;
using NoticeBoard.Infrastructure;

namespace NoticeBoard.Controllers
{
    public class PostsController : Controller
    {
        private readonly INoticeBoardRepository _repository;

        public PostsController(INoticeBoardRepository repository)
        {
            _repository = repository;
        }

        // GET: Posts
        public async Task<IActionResult> Index(string postCategory, string searchString, string sortOrder, int? page)
        {
            if (_repository.Posts() == null)
            {
                return Problem("Entity set 'NoticeBoard.Post'  is null.");
            }
            PostsViewModel? viewModel = null;
            try
            {

                int pageSize = 8;
                int pageNumber = page.HasValue && page.Value > 0 ? page.Value : 1;
                //페이지값이 널인경우 1을 반환 1보다 안작아서 페이지값가져옴 페이지가 널이아니면 페이지값반환 1보다 작을시 1을 반환

                // Use LINQ to get list of genres.
                IQueryable<string> categoryQuery = from m in _repository.Posts()
                                                   orderby m.Category
                                                   select m.Category;

                var posts = from m in _repository.Posts()
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
                var categories = await _repository.SelectAsync();

                viewModel = new PostsViewModel
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return View(viewModel);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if ( _repository.Posts() == null)
            {
                return NotFound();
            }

            var post = await _repository.FindAsync(id);

            post.Views++;
            //_context.Update(post);
            await _repository.SaveChangesAsync();

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
            var categories = await _repository.SelectAsync();

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Post post, ICollection<IFormFile>? Files = null)
        {
            DateTime datetime = DateTime.Now;
            post.LastUpdated = datetime;
            await _repository.AddAsync(post);
            await _repository.SaveChangesAsync();

            if (Files?.Count > 0)
            {
                var files = Request.Form.Files;
                var uploadPath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Files");
                for (var i = 0; i < Files.Count; i++)
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
                   await _repository.AttachAsync(attachFile); //이미 데이터베이스에 있는 엔터티를 수정하려는 경우 사용
                }
            }

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var uploadPath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Files");
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
            try
            {
                var post = await _repository.FindPostAsync(id);

                if (await _repository.FirstOrDefaultAsyncA(id) != null) ///?
                {
                    var categories = await _repository.ListAsync();
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

                    var categories = await _repository.ListAsync();
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View(null);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetFiles(int id)
        {
            var files = await _repository.FindList(id); //변수가 Task<List<AttachFile>> 객체 비동기적 호출
            var result = files.Select(p => new
            {
                FileId = p.FileId,
                FileName = p.FileName,
                FileSize = p.FileData.Length
            });
            Console.WriteLine(Json(result).Value);
            return Json(result);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Post post, ICollection<IFormFile> Files, [FromForm] string? FileId = null )
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //editor의 사진이 변경된경우 
                    var rootPath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Files");

                    var beforePost = await _repository.FindAsync(id);
                    var beforeContent = beforePost.Content;
                    var regexB = new Regex("<img[^>]+src\\s*=\\s*['\"]([^'\"]+)['\"][^>]*>");
                    var matchB = regexB.Match(beforeContent);
                    var imageUrlB = matchB.Groups[1].Value;
                    var imageNameB = Path.GetFileName(imageUrlB);
                    var imagePathB = Path.Combine(rootPath, imageNameB);

                    var content = post.Content;
                    var regex = new Regex("<img[^>]+src\\s*=\\s*['\"]([^'\"]+)['\"][^>]*>");
                    var match = regex.Match(content);
                    var imageUrl = match.Groups[1].Value;
                    var imageName = Path.GetFileName(imageUrl);

                    if (imageNameB != imageName)
                    {
                        if (System.IO.File.Exists(imagePathB))
                        {
                            System.IO.File.Delete(imagePathB);
                        }
                    }
                    var Post = await _repository.FirstOrDefaultAsyncP(id);
                    DateTime datetime = DateTime.Now;
                    Post.Nickname = post.Nickname;
                    Post.Title = post.Title;
                    Post.Content = post.Content;
                    Post.LastUpdated = datetime;
                    Post.Category = post.Category;
                   await _repository.UpdateAsync(Post);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await PostExists(post.PostId))
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
            if (Files.Count > 0)
            {
                var uploadPath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Files");
                var files = Request.Form.Files;
                try
                {
                    for (var i = 0; i < Files.Count; i++)
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

                       await _repository.AttachAsync(attachFile);
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
                    var filePaths = await _repository.SelectByFiledId(intFileId);
                    foreach (var filePath in filePaths)
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    var deleteAttachFileId = await _repository.FindAsyncA(intFileId); //Id를 반환하는 건가.. 변수 이름 왜이럼
                    if (deleteAttachFileId != null)
                    {
                      await  _repository.RemoveAttachFile(deleteAttachFileId);///?
                    }
                    await _repository.SaveChangesAsync();
                }
            }

            return RedirectToAction("Details", new { id = id });
        }


        [HttpDelete]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_repository.Posts() == null)
            {
                return Problem("Entity set 'NoticeBoardContext.Post'  is null.");
            }
            var filePaths = await _repository.SelectByPostId(id);

            foreach (var filePath in filePaths)
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            var attachfiles = await _repository.FindListAsyncA(id);

            foreach (var attachfile in attachfiles)
            {
               await _repository.RemoveAttachFile(attachfile);
            }

            await _repository.RemoveComments(id);

            var post = await _repository.FindAsyncP(id);

            var content = post.Content;
            var regex = new Regex("<img[^>]+src\\s*=\\s*['\"]([^'\"]+)['\"][^>]*>");
            var match = regex.Match(content);
            var rootPath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Files");
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
                _repository.RemovePost(post);
            }

            await _repository.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteIds(string id)
        {
            string[] splitId = id.Split(",");

            if (_repository.Posts() == null)
            {
                return BadRequest();
            }

            bool Remove = false;

            for (int i = 0; i < splitId.Length; i++)
            {
                var intId = int.Parse(splitId[i]);
                var filePaths = await _repository.SelectByPostId(intId);

                foreach (var filePath in filePaths)
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                var attachfiles = await _repository.FindListAsyncA(intId);

                foreach (var attachfile in attachfiles)
                {
                   await _repository.RemoveAttachFile(attachfile);
                }

                var comments = _repository.RemoveComments(intId);

                var post = await _repository.FindAsyncP(intId);

                var content = post.Content;
                var regex = new Regex("<img[^>]+src\\s*=\\s*['\"]([^'\"]+)['\"][^>]*>");
                var match = regex.Match(content);
                var rootPath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "Files");
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
                    await _repository.RemovePost(post);
                    Remove = true;
                }
            }

            if (Remove)
                await _repository.SaveChangesAsync();

            return Ok();
        }
        // POST: Posts/Detail/:postId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, [Bind("CommentId,PostId,Content")] Comment comment)
        {
            var post = await _repository.FindAsyncP(id);

            if (post == null)
            {
                return NotFound();
            }

            DateTime datetime = DateTime.Now;
            comment.LastUpdated = datetime;
            //comment.PostId = id;
            await _repository.UpdateAsyncComment(comment);

            await _repository.SaveChangesAsync();
            return RedirectToAction("Details", new { id = id });
            //}
        }

        // POST: Posts/DeleteC/5
        [HttpPost, ActionName("DeleteC")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteC(int id)
        {
            if (_repository.Comments() == null)
            {
                return Problem("Entity set 'NoticeBoardContext.Comments'  is null.");
            }
            var comment = await _repository.FIndAsyncComment(id);
            if (comment != null)
            {
               await _repository.RemoveComment(comment);
            }

            await _repository.SaveChangesAsync();
            return RedirectToAction("Details", new { id = comment.PostId });
        }

        private Task<bool> PostExists(int id)
        {
            return _repository.PostExistsAsync(id);
        }
    }
}
