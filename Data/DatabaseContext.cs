using Microsoft.EntityFrameworkCore;
using MultimediaLibrary.Models;

namespace MultimediaLibrary.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Gallery> Galleries { get; set; } = null!;
        public DbSet<Media> Media { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"server=localhost\sqlexpress;database=MultimediaLibrary;TrustServerCertificate=True;Integrated Security=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasMany(u => u.Following)
                .WithMany(u => u.Followers)
                .UsingEntity<Dictionary<ulong, object>>(
                    "UserFollowers",
                    r => r.HasOne<User>().WithMany().HasForeignKey("FollowingUserId"),
                    l => l.HasOne<User>().WithMany().HasForeignKey("FollowerUserId"),
                    je => je.HasKey("FollowingUserId", "FollowerUserId")
                );

            modelBuilder.Entity<Media>()
                .HasMany(m => m.Tags)
                .WithMany(t => t.Media)
                .UsingEntity<Dictionary<ulong, object>>(
                    "MediaTags",
                    r => r.HasOne<Tag>().WithMany().HasForeignKey("TagId").OnDelete(DeleteBehavior.ClientCascade),
                    l => l.HasOne<Media>().WithMany().HasForeignKey("MediaUuid").OnDelete(DeleteBehavior.Cascade),
                    je => je.HasKey("TagId", "MediaUuid")
                );

            modelBuilder.Entity<Gallery>()
                .HasMany(g => g.Media)
                .WithMany(m => m.Galleries)
                .UsingEntity<Dictionary<ulong, object>>(
                    "GalleryMedia",
                    r => r.HasOne<Media>().WithMany().HasForeignKey("MediaUuid").OnDelete(DeleteBehavior.ClientCascade),
                    l => l.HasOne<Gallery>().WithMany().HasForeignKey("GalleryId").OnDelete(DeleteBehavior.Cascade),
                    je => je.HasKey("MediaUuid", "GalleryId")
                );
        }
    }
}
