using CommentingSystem.Data;
using CommentingSystem.Domain;
using CommentingSystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommentingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly CommentingSystemContext _db;

        public HomeController(CommentingSystemContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {

            List<Comment> comments = await _db.Comments
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.Children)
                .ToListAsync();

            // Structure comments into a tree
            List<Comment> rootComments = comments
                .Where(c => c.ParentId == null)
                .AsParallel()
                .ToList();

            return View(rootComments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentCreationDto commentCDto)
        {
            if (ModelState.IsValid)
            {
                var newComment = new Comment
                {
                    ParentId = commentCDto.ReplyToCommentId,
                    FullName = commentCDto.FullName,
                    Email = commentCDto.Email,
                    Content = commentCDto.Content,
                    DateCreated = DateTimeOffset.Now,
                    DateModified = DateTimeOffset.Now
                };

                await _db.Comments.AddAsync(newComment);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // TODO: Throw some error
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int CommentId)
        {
            var comment = await _db.Comments
                .Include(x => x.Children)
                .SingleOrDefaultAsync(x => x.CommentId == CommentId);

            if (comment == null)
            {
                return NotFound();
            }

            foreach (var child in comment.Children)
            {
                child.ParentId = null;
            }

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
