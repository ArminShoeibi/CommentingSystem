using System.Net;

namespace CommentingSystem.Domain
{
    public class Like
    {
        public int LikeId { get; set; }
        public int CommentId { get; set; }
        public IPAddress IP { get; set; }

        #region Navigation Properties
        public Comment Comment { get; set; }
        #endregion
    }
}
