using System.ComponentModel.DataAnnotations;

namespace CommentingSystem.DTOs;
public record class CreateCommentDto
{
    public int? ReplyToCommentId { get; init; }

    [Required, StringLength(1000, MinimumLength = 10)]
    public string Content { get; init; }

    [EmailAddress, StringLength(100, MinimumLength = 6), Required]
    public string Email { get; init; }

    [Required, StringLength(60, MinimumLength = 3)]
    public string FullName { get; init; }
}

