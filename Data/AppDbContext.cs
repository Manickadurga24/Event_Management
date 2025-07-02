using EventManagementUpdatedProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion; // For enum conversion

namespace EventManagementUpdatedProject.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
       // public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // MUST CALL THIS FOR IDENTITY

            // Configure the UserRole enum conversion to int
            modelBuilder.Entity<AppUser>()
                .Property(au => au.Type)
                .HasConversion<string>();

            //modelBuilder.Entity<Ticket>()
            //.HasOne(t => t.User)
            //.WithMany()
            //.HasForeignKey(t => t.UserID)
            //.OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Ticket>()
            //    .HasOne(t => t.Event)
            //    .WithMany()
            //    .HasForeignKey(t => t.EventID);
        }
    }
}
