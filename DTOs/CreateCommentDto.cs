using System.ComponentModel.DataAnnotations;

namespace CommentingSystem.DTOs
{
    public record CreateCommentDto
    {
        public int? ReplyToCommentId { get; init; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(1000, MinimumLength = 10)]
        public string Content { get; init; }

        [EmailAddress]
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 6)]
        public string Email { get; init; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(60, MinimumLength = 3)]
        public string FullName { get; init; }
    }
}
