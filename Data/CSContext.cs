using CommentingSystem.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommentingSystem.Data
{
    public class CSContext : DbContext
    {
        public CSContext(DbContextOptions<CSContext> dbContextOptions) : base(dbContextOptions)
        {
            
        }
        public DbSet<Comment> Comments { get; set; }

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
    }
}
