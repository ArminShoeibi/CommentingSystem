using System;
using System.Collections.Generic;

namespace CommentingSystem.Domain
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int? ParentId  { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }



        #region Navigation Properties

        public Comment Parent { get; set; }
        public ICollection<Comment> Children { get; set; }

        #endregion
    }
}
