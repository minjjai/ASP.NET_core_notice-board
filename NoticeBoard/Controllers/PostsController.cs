using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NoticeBoard.Data;
using NoticeBoard.Models;

namespace NoticeBoard.Controllers
{
    //종속성 주입
    public class PostsController : Controller
    {
        private readonly NoticeBoardContext _context;

        public PostsController(NoticeBoardContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index(string postCategory, string searchString, string sortOrder)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'NoticeBoard.Post'  is null.");
            }

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
                case "PastOrder":
                    posts = posts.OrderByDescending(s => s.LastUpdated);
                    break;
                default:
                    posts = posts.OrderBy(s => s.LastUpdated);
                    break;
            }

            var postCategoryVM = new PostCategoryViewModel
            {
                Categories = new SelectList(await categoryQuery.Distinct().ToListAsync()),

                Posts = await posts.ToListAsync()
            };

            return View(postCategoryVM);
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
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nickname,Title,Content,Category")] Post post)
        {
            if (ModelState.IsValid)
            {
                DateTime datetime = DateTime.Now;
                post.LastUpdated = datetime;

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            //if (id == null || _context.Posts == null)
            //{
            //    return NotFound();
            //}

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Nickname,Title,Content,Category")] Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DateTime datetime = DateTime.Now;
                    post.LastUpdated = datetime;
                    _context.Update(post);
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
            return RedirectToAction("Details", new { id = id });
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'NoticeBoardContext.Post'  is null.");
            }

            var comments = _context.Comments.Where(c => c.PostId == id);
            _context.Comments.RemoveRange(comments);
            var post = await _context.Posts.FindAsync(id);

            if (post != null)
            {
                _context.Posts.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
