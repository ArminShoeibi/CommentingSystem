using CommentingSystem.Domain;
using Microsoft.EntityFrameworkCore;

namespace CommentingSystem.Data;

public class CommentingSystemDbContext : DbContext
{
    public CommentingSystemDbContext(DbContextOptions<CommentingSystemDbContext> dbContextOptions) : base(dbContextOptions)
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

            comment.Property(c => c.FullName).HasMaxLength(60).IsRequired();
            comment.Property(c => c.Email).HasMaxLength(100).IsRequired();
            comment.Property(c => c.Content).HasMaxLength(1000).IsRequired();
        });
    }

    public DbSet<Comment> Comments { get; set; }
}
