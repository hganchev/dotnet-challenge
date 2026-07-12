using Fundamentals.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fundamentals.Data;

/// <summary>
/// One <see cref="DbContext"/> per bounded context. In ASP.NET Core this would
/// be registered as Scoped (one instance per HTTP request) — never Singleton,
/// since it is not thread-safe and accumulates tracked-entity state.
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Genre> Genres => Set<Genre>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API configuration for Book — equivalent to what data annotations
        // express on Author, but written entirely in OnModelCreating.
        modelBuilder.Entity<Book>(book =>
        {
            book.ToTable("Books");
            book.HasKey(b => b.Id);
            book.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(150);
            book.Property(b => b.Price)
                .HasColumnType("decimal(8,2)");

            book.HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optimistic-concurrency token: EF Core includes Version in the
            // WHERE clause of generated UPDATE/DELETE statements. SQLite has no
            // database-generated rowversion, so the app increments it manually.
            book.Property(b => b.Version).IsConcurrencyToken();

            book.HasOne(b => b.Genre)
                .WithMany()
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Static lookup data seeded once, at migration-build time — not with
        // ad-hoc runtime inserts (see MigrationsDemo for the anti-pattern this avoids).
        modelBuilder.Entity<Genre>(genre =>
        {
            genre.ToTable("Genres");
            genre.HasKey(g => g.Id);
            genre.Property(g => g.Name).IsRequired().HasMaxLength(50);
            genre.HasData(
                new Genre { Id = 1, Name = "Non-Fiction" },
                new Genre { Id = 2, Name = "Science Fiction" },
                new Genre { Id = 3, Name = "Biography" });
        });
    }
}
