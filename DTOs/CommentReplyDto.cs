using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommentingSystem.DTOs
{
    public class CommentReplyDto : CommentCreationDto
    {
        public int ReplyToCommentId { get; set; }
    }
}
