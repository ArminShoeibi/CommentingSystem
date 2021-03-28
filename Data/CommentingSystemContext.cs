using CommentingSystem.Domain;
using Microsoft.EntityFrameworkCore;

namespace CommentingSystem.Data
{
    public class CommentingSystemContext : DbContext
    {
        public CommentingSystemContext(DbContextOptions<CommentingSystemContext> dbContextOptions) : base(dbContextOptions)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comment>(comment =>
            {
                comment.HasKey(c => c.CommentId);
                comment.HasIndex(c => c.ParentId);

                comment.HasOne(c => c.Parent)
                       .WithMany(c => c.Children)
                       .HasForeignKey(c => c.ParentId);

                comment.HasMany(c => c.Likes)
                        .WithOne(c => c.Comment)
                        .HasForeignKey(c => c.CommentId)
                        .OnDelete(DeleteBehavior.Cascade);

                comment.Property(c => c.FullName).HasMaxLength(60).IsRequired();
                comment.Property(c => c.Email).HasMaxLength(100).IsRequired();
                comment.Property(c => c.Content).HasMaxLength(1000).IsRequired();
            });

            modelBuilder.Entity<Like>(like =>
            {
                like.HasKey(c => c.Id);
                like.Property(c => c.Ip).HasMaxLength(16).IsRequired();
            });
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
    }
}
