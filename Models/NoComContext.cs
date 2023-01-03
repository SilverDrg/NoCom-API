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
            modelBuilder.HasDefaultSchema("NoCom");
            base.OnModelCreating(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
