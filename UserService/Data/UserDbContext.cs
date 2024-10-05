using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UserService.Models;

namespace UserService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Kreiraj početnog korisnika (admin)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,  // Obavezno postavi ID jer je primarni ključ
                    Name = "admin",
                    Email = "admin@example.com",
                    Password = "admin"  // Dodaj inicijalnu lozinku
                }
            );
        }
    }
}
