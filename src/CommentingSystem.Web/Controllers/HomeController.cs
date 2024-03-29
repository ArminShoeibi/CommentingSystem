﻿using CommentingSystem.Data;
using CommentingSystem.Domain;
using CommentingSystem.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommentingSystem.Controllers;

public class HomeController : Controller
{
    private readonly CommentingSystemDbContext _db;

    public HomeController(CommentingSystemDbContext db)
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
    public async Task<IActionResult> CreateComment(CreateCommentDto commentCDto)
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
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var comments = await _db.Comments
            .Include(x => x.Children).ToListAsync();

        var flatten = Flatten(comments.Where(x => x.CommentId == commentId));

        _db.Comments.RemoveRange(flatten);

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    IEnumerable<Comment> Flatten(IEnumerable<Comment> comments) =>
        comments.SelectMany(x => Flatten(x.Children)).Concat(comments);
}
