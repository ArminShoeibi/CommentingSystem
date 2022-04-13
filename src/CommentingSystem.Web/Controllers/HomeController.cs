using CommentingSystem.Data;
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
            .Include(c => c.Likes)
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
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var comments = await _db.Comments
            .Include(x => x.Children).ToListAsync();

        var flatten = Flatten(comments.Where(x => x.CommentId == commentId));

        _db.Comments.RemoveRange(flatten);

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<JsonResult> LikeComment(int commentId)
    {
        if (!await _db.Comments.AnyAsync(c => c.CommentId == commentId))
            return Json(new { Status = "failed", message = "comment id is not correct!" });
        var userIp = HttpContext.Connection.RemoteIpAddress.ToString();
        //if once user like , after he cant unlike
        //if(await _db.Likes.AnyAsync(c => c.CommentId == commentId && c.Ip == userIp))
        //{
        //    return Json(new { Status = "failed", message = "you have alredy voted for this comment once" });
        //}

        //if user like a comment, then can unlike comment
        var likeObj = await _db.Likes.FirstOrDefaultAsync(c => c.CommentId == commentId && c.Ip == userIp);
        if (likeObj!=null)
        {
            _db.Likes.Remove(likeObj);
        }
        else
        {
            await _db.Likes.AddAsync(new Like()
            {
                CommentId = commentId,
                Ip = userIp
            });
        }
       
        await _db.SaveChangesAsync();
        return Json(new { Status = "success", message = await _db.Likes.CountAsync(c => c.CommentId == commentId) });
    }

    IEnumerable<Comment> Flatten(IEnumerable<Comment> comments) =>
        comments.SelectMany(x => Flatten(x.Children)).Concat(comments);
}
