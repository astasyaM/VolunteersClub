using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using VolunteersClub.Models;

namespace VolunteersClub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        }

        public DbSet<Event> Events {  get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Leader> Leaders { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Responsibility> Responsibilities {  get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<VolunteerStatus> VolunteerStatuses { get; set; }
    }
}
