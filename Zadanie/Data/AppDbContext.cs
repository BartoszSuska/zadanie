using Microsoft.EntityFrameworkCore;
using Zadanie.Models;

namespace Zadanie.Data {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Contact> Contacts { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Subcategory> Subcategories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            //ekskluzywny email dla każdego kontaktu
            modelBuilder.Entity<Contact>()
                .HasIndex(c => c.Email)
                .IsUnique();

            //każda kategoria może mieć wiele podkategorii, ale każda podkategoria należy do jednej kategorii
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Subcategories)
                .WithOne(s => s.Category)
                .HasForeignKey(s => s.CategoryId);

            //każdy kontakt należy do jednej kategorii
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Category)
                .WithMany()
                .HasForeignKey(c => c.CategoryId);

            //każdy kontakt może mieć jedną podkategorię (opcjonalnie), ale każda podkategoria może być przypisana do wielu kontaktów
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Subcategory)
                .WithMany()
                .HasForeignKey(c => c.SubcategoryId);

            //Seed podstawowych danych do bazy danych - kategorie, podkategorie i kontakty
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Służbowy" },
                new Category { Id = 2, Name = "Prywatny" },
                new Category { Id = 3, Name = "Inny" }
            );

            modelBuilder.Entity<Subcategory>().HasData(
                new Subcategory { Id = 1, Name = "Szef", CategoryId = 1 },
                new Subcategory { Id = 2, Name = "Klient", CategoryId = 1 }
            );

            modelBuilder.Entity<Contact>().HasData(
                new Contact {
                    Id = 1,
                    Name = "Jan",
                    Surname = "Kowalski",
                    Email = "jan.kowalski@example.com",
                    Password = "Haslo123!",
                    CategoryId = 1,
                    SubcategoryId = 2,
                    Phone = "123456789",
                    BirthDate = new DateOnly(1990, 1, 1)
                },
                new Contact {
                    Id = 2,
                    Name = "Anna",
                    Surname = "Nowak",
                    Email = "anna.nowak@example.com",
                    Password = "Haslo123!",
                    CategoryId = 2,
                    SubcategoryId = null,
                    Phone = "987654321",
                    BirthDate = new DateOnly(1995, 5, 15)
                }
            );
        }
    }
}
