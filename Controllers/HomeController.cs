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
        private readonly CSContext _db;

        public HomeController(CSContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> Index()
        {

            List<Comment> comments = await _db.Comments.AsNoTrackingWithIdentityResolution()
                                                        .Include(c => c.Children)
                                                        .ToListAsync();

            // Structure Comments into a tree
            List<Comment> commentsTree = comments.Where(c => c.ParentId == null)
                                                 .AsParallel()
                                                 .ToList();

            return View(commentsTree);
        }


        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentCreationDto commentCDto)
        {
            if (ModelState.IsValid)
            {
                Comment newComment = new()
                {
                    DateCreated = DateTimeOffset.Now,
                    DateModified = DateTimeOffset.Now,
                    Content = commentCDto.Content,
                    FullName = commentCDto.FullName,
                    Email = commentCDto.Email,
                };

                await _db.Comments.AddAsync(newComment);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // TODO: Throw some error
            return RedirectToAction(nameof(Index));


        }


        [HttpGet]
        public IActionResult CreateReply(int replyToCommentId)
        {
            return PartialView(new CommentReplyDto() { ReplyToCommentId = replyToCommentId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateReply(CommentReplyDto commentRDto)
        {
            if (ModelState.IsValid)
            {
                Comment newReply = new()
                {
                    DateCreated = DateTimeOffset.Now,
                    DateModified = DateTimeOffset.Now,
                    ParentId = commentRDto.ReplyToCommentId,
                    Email = commentRDto.Email,
                    FullName = commentRDto.FullName,
                    Content = commentRDto.Content,
                };
                await _db.Comments.AddAsync(newReply);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // TODO: Throw some error
            return RedirectToAction(nameof(Index));
        }
    }
}
