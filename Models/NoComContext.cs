using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NoCom_API.Models
{
    public partial class NoComContext : DbContext
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
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Session> Sessions { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UsersRole> UsersRoles { get; set; } = null!;
        public virtual DbSet<Website> Websites { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comments", "nocom");

                entity.HasComment("Comments from users");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Comment1).HasColumnName("comment");

                entity.Property(e => e.Likes).HasColumnName("likes");

                entity.Property(e => e.Nsfw).HasColumnName("nsfw");

                entity.Property(e => e.ReplyTo).HasColumnName("reply_to");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.WebsiteId).HasColumnName("website_id");

                entity.HasOne(d => d.ReplyToNavigation)
                    .WithMany(p => p.InverseReplyToNavigation)
                    .HasForeignKey(d => d.ReplyTo)
                    .HasConstraintName("fk_comments_comments");

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
                entity.ToTable("event_logs", "nocom");

                entity.HasComment("Save events such as log in, log out and others");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Event).HasColumnName("event");

                entity.Property(e => e.EventTime)
                    .HasColumnName("event_time")
                    .HasDefaultValueSql("CURRENT_TIME");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.EventLogs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_event_logs_users");
            });

            modelBuilder.Entity<LikedComment>(entity =>
            {
                entity.ToTable("liked_comments", "nocom");

                entity.HasComment("This table is used for keeping track of likes on comments");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CommentId).HasColumnName("comment_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

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

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles", "nocom");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(100)
                    .HasColumnName("role_name");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.ToTable("sessions", "nocom");

                entity.HasComment("Keeps track of user sessions and session tokens");

                entity.HasIndex(e => e.UserId, "unq_sessions_user_id")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.RefreshToken)
                    .HasColumnType("character varying")
                    .HasColumnName("refresh_token");

                entity.Property(e => e.RefreshTokenEnd)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("refresh_token_end");

                entity.Property(e => e.SessionEnd)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("session_end");

                entity.Property(e => e.SessionToken)
                    .HasColumnType("character varying")
                    .HasColumnName("session_token");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Session)
                    .HasForeignKey<Session>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sessions_users");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users", "nocom");

                entity.HasComment("user information");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.BannerImage).HasColumnName("banner_image");

                entity.Property(e => e.Email)
                    .HasMaxLength(128)
                    .HasColumnName("email");

                entity.Property(e => e.IsAdmin).HasColumnName("is_admin");

                entity.Property(e => e.Password)
                    .HasMaxLength(128)
                    .HasColumnName("password");

                entity.Property(e => e.ProfileImage).HasColumnName("profile_image");

                entity.Property(e => e.Username)
                    .HasMaxLength(128)
                    .HasColumnName("username");
            });

            modelBuilder.Entity<UsersRole>(entity =>
            {
                entity.ToTable("users_role", "nocom");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.WebsiteId).HasColumnName("website_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UsersRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_role_roles");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_roles_users");

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.UsersRoles)
                    .HasForeignKey(d => d.WebsiteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_roles_websites");
            });

            modelBuilder.Entity<Website>(entity =>
            {
                entity.ToTable("websites", "nocom");

                entity.HasComment("Saving websites without keeping track what kind of websites the users are browsing on");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.UrlHash)
                    .HasMaxLength(128)
                    .HasColumnName("url_hash");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
