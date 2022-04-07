using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NoCom_API.Models
{
    public partial class NoComContext : IdentityDbContext<IdentityUser>
    {
        public NoComContext()
        {
        }

        public NoComContext(DbContextOptions<NoComContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<EventLog> EventLogs { get; set; } = null!;
        public virtual DbSet<LikedComment> LikedComments { get; set; } = null!;
        public virtual DbSet<Website> Websites { get; set; } = null!;
        public virtual DbSet<Profile> Profile { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("NoComTest");
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Comment>(entity =>
            //{
            //    entity.ToTable("comments", "nocom");

            //    entity.HasComment("Comments from users");

            //    entity.Property(e => e.Id)
            //        .ValueGeneratedNever()
            //        .HasColumnName("id");

            //    entity.Property(e => e.Content).HasColumnName("comment");

            //    entity.Property(e => e.Likes).HasColumnName("likes");

            //    entity.Property(e => e.Nsfw).HasColumnName("nsfw");

            //    entity.Property(e => e.ReplyTo).HasColumnName("reply_to");

            //    entity.Property(e => e.UserId).HasColumnName("user_id");

            //    entity.Property(e => e.WebsiteId).HasColumnName("website_id");

            //    entity.HasOne(d => d.ReplyToNavigation)
            //        .WithMany(p => p.InverseReplyToNavigation)
            //        .HasForeignKey(d => d.ReplyTo)
            //        .HasConstraintName("fk_comments_comments");

            //    entity.HasOne(d => d.User)
            //        .WithMany(p => p.Comments)
            //        .HasForeignKey(d => d.UserId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_comments_users");

            //    entity.HasOne(d => d.Website)
            //        .WithMany(p => p.Comments)
            //        .HasForeignKey(d => d.WebsiteId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_comments_websites");
            //});

            //modelBuilder.Entity<EventLog>(entity =>
            //{
            //    entity.ToTable("event_logs", "nocom");

            //    entity.HasComment("Save events such as log in, log out and others");

            //    entity.Property(e => e.Id)
            //        .ValueGeneratedNever()
            //        .HasColumnName("id");

            //    entity.Property(e => e.Event).HasColumnName("event");

            //    entity.Property(e => e.EventTime)
            //        .HasColumnName("event_time")
            //        .HasDefaultValueSql("CURRENT_TIME");

            //    entity.Property(e => e.UserId).HasColumnName("user_id");

            //    entity.HasOne(d => d.User)
            //        .WithMany(p => p.EventLogs)
            //        .HasForeignKey(d => d.UserId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_event_logs_users");
            //});

            //modelBuilder.Entity<LikedComment>(entity =>
            //{
            //    entity.ToTable("liked_comments", "nocom");

            //    entity.HasComment("This table is used for keeping track of likes on comments");

            //    entity.Property(e => e.Id)
            //        .ValueGeneratedNever()
            //        .HasColumnName("id");

            //    entity.Property(e => e.CommentId).HasColumnName("comment_id");

            //    entity.Property(e => e.UserId).HasColumnName("user_id");

            //    entity.HasOne(d => d.Comment)
            //        .WithMany(p => p.LikedComments)
            //        .HasForeignKey(d => d.CommentId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_users_comments_comments");

            //    entity.HasOne(d => d.User)
            //        .WithMany(p => p.LikedComments)
            //        .HasForeignKey(d => d.UserId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("fk_users_comments_users");
            //});

            //modelBuilder.Entity<Website>(entity =>
            //{
            //    entity.ToTable("websites", "nocom");

            //    entity.HasComment("Saving websites without keeping track what kind of websites the users are browsing on");

            //    entity.Property(e => e.Id)
            //        .ValueGeneratedNever()
            //        .HasColumnName("id");

            //    entity.Property(e => e.UrlHash)
            //        .HasMaxLength(128)
            //        .HasColumnName("url_hash");
            //});

            //OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
