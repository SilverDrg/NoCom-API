using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NoCom_API.Models
{
    public partial class CoreDbContext : DbContext
    {
        public CoreDbContext()
        {
        }

        public CoreDbContext(DbContextOptions<CoreDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<EventLog> EventLogs { get; set; } = null!;
        public virtual DbSet<LikedComment> LikedComments { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Website> Websites { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=silver;Database=NoCom;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasComment("Comments from users");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_comments_users");

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.WebsiteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_comments_websites");
            });

            modelBuilder.Entity<EventLog>(entity =>
            {
                entity.HasComment("Save events such as log in, log out and others");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EventTime).HasDefaultValueSql("CURRENT_TIME");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.EventLogs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_event_logs_users");
            });

            modelBuilder.Entity<LikedComment>(entity =>
            {
                entity.HasComment("This table is used for keeping track of likes on comments");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Comment)
                    .WithMany(p => p.LikedComments)
                    .HasForeignKey(d => d.CommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_comments_comments");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.LikedComments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_comments_users");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasComment("user information");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Website>(entity =>
            {
                entity.HasComment("Saving websites without keeping track what kind of websites the users are browsing on");

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
