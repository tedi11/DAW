using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProiectDAW.Models;

namespace ProiectDAW.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>
        options)
        : base(options)
        {
        }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Project> Projects{get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Member> Members { get; set; }
        

        protected override void OnModelCreating(ModelBuilder
        modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // definire primary key compus (Member)
            modelBuilder.Entity<Member>()
            .HasKey(ac => new { 
                ac.Id,
                ac.UserId, 
                ac.ProjectId });
            // definire relatii cu modelele User si Project (FK)
            modelBuilder.Entity<Member>()
            .HasOne(ac => ac.User)
            .WithMany(ac => ac.Members)
            .HasForeignKey(ac => ac.UserId);

            modelBuilder.Entity<Member>()
            .HasOne(ac => ac.Project)
            .WithMany(ac => ac.Members)
            .HasForeignKey(ac => ac.ProjectId);
        }
    }


}

