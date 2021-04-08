using CommentingSystem.Data;
using CommentingSystem.Domain;
using CommentingSystem.DTOs;
using CommentingSystem.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommentingSystem.Controllers
{
    public class CommentsController : Controller
    {

        private readonly CommentingSystemContext _db;

        public CommentsController(CommentingSystemContext db)
        {
            _db = db;
        }

        // /comments/findcomment/6
        public IActionResult FindComment(int id)
        {
            var commentWithAllOfItsDescendants =
                    _db.Comments
                            .Include(comment => comment.Children)
                            .Where(comment => comment.CommentId == id
                                            || comment.Children.Any(m => comment.CommentId == m.ParentId))
                            .ToList()
                            .FirstOrDefault(comment => comment.CommentId == id);

            return Json(commentWithAllOfItsDescendants);
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
        public async Task<IActionResult> CreateComment(CreateCommentDto createCommentDto)
        {
            if (ModelState.IsValid)
            {
                var newComment = new Comment
                {
                    ParentId = createCommentDto.ParentId,
                    FullName = createCommentDto.FullName,
                    Email = createCommentDto.Email,
                    Content = createCommentDto.Content,
                };

                await _db.Comments.AddAsync(newComment);
                await _db.SaveChangesAsync();

                if (createCommentDto.ParentId.HasValue && createCommentDto.ParentId.Value > 0)
                {
                    Comment parentComment = await _db.Comments.FindAsync(createCommentDto.ParentId);

                    List<int> childrenIdsOfParent = new();

                    if (string.IsNullOrEmpty(parentComment.ChildrenIds))
                    {
                        childrenIdsOfParent.Add(newComment.CommentId);
                    }
                    else
                    {
                        List<int> currentChildrenIdsOfParent = parentComment.ChildrenIds.CommaSeparatedStringToIntList();

                        childrenIdsOfParent.AddRange(currentChildrenIdsOfParent);
                        childrenIdsOfParent.Add(newComment.CommentId);
                    }

                    parentComment.ChildrenIds = string.Join<int>(",", childrenIdsOfParent);
                    await _db.SaveChangesAsync();

                }

                return RedirectToAction(nameof(Index));
            }

            // TODO: Throw some error
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            Comment commentToDelete = await _db.Comments.FindAsync(commentId);

            if (string.IsNullOrEmpty(commentToDelete.ChildrenIds))
            {
                _db.Comments.Remove(commentToDelete);
            }
            else
            {
                List<int> currentChildrenIdsOfParent = commentToDelete.ChildrenIds.CommaSeparatedStringToIntList();

                List<Comment> childCommentsToDelete =
                    await _db.Comments.Where(c => currentChildrenIdsOfParent.Contains(c.CommentId))
                                      .ToListAsync();
                childCommentsToDelete.Add(commentToDelete);

                _db.Comments.RemoveRange(childCommentsToDelete);
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<JsonResult> LikeComment(int commentId)
        {
            if (!await _db.Comments.AnyAsync(c => c.CommentId == commentId))
                return Json(new { Status = "failed", message = "comment id is not correct!" });
            var userIP = HttpContext.Connection.RemoteIpAddress;
            //if once user like , after he cant unlike
            //if(await _db.Likes.AnyAsync(c => c.CommentId == commentId && c.Ip == userIp))
            //{
            //    return Json(new { Status = "failed", message = "you have alredy voted for this comment once" });
            //}

            //if user like a comment, then can unlike comment
            var likeObj = await _db.Likes.FirstOrDefaultAsync(c => c.CommentId == commentId && c.IP == userIP);
            if (likeObj != null)
            {
                _db.Likes.Remove(likeObj);
            }
            else
            {
                Like newLike = new()
                {
                    CommentId = commentId,
                    IP = userIP
                };
                await _db.Likes.AddAsync(newLike);
            }

            await _db.SaveChangesAsync();
            return Json(new { Status = "success", message = await _db.Likes.CountAsync(c => c.CommentId == commentId) });
        }
    }
}
