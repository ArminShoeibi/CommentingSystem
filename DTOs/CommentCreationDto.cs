using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CommentingSystem.DTOs;

public record CommentCreationDto(
     int? ReplyToCommentId,
     [Required, StringLength(1000, MinimumLength = 10)] string Content,
     [EmailAddress, StringLength(100, MinimumLength = 6), Required] string Email,
     [Required, StringLength(60, MinimumLength = 3)] string FullName);
