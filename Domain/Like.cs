namespace CommentingSystem.Domain
{
    public class Like
    {
        public int Id { get; set; }
        public string Ip { get; set; }

        #region navigation property
        public int CommentId { get; set; }
        public Comment Comment { get; set; }
        #endregion
    }
}
