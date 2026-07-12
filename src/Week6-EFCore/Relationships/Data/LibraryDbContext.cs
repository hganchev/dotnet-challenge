using Microsoft.EntityFrameworkCore;
using Relationships.Entities;

namespace Relationships.Data;

/// <summary>
/// Wires up every relationship kind covered this week: one-to-one
/// (Author/AuthorProfile), one-to-many (Author/Book), many-to-many via an
/// explicit join entity with a payload (Book/Tag/BookTag), and an owned
/// value object (AuthorProfile.Address).
/// </summary>
public class LibraryDbContext : DbContext
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<AuthorProfile> AuthorProfiles => Set<AuthorProfile>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<BookTag> BookTags => Set<BookTag>();

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // One-to-one: AuthorProfile's PK is also its FK to Author.
        modelBuilder.Entity<AuthorProfile>(profile =>
        {
            profile.HasKey(p => p.AuthorId);
            profile.HasOne(p => p.Author)
                .WithOne(a => a.Profile)
                .HasForeignKey<AuthorProfile>(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Owned value object — mapped to columns on the AuthorProfiles table.
            profile.OwnsOne(p => p.Address, address =>
            {
                address.Property(a => a.Street).HasMaxLength(200);
                address.Property(a => a.City).HasMaxLength(100);
                address.Property(a => a.PostalCode).HasMaxLength(20);
            });
        });

        // One-to-many: an Author's Books are deleted with the Author (Cascade
        // is appropriate here since Books have no meaning without their Author).
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-many via explicit join entity (BookTag) because the
        // relationship carries a payload (AddedAt). Tags must survive even if
        // a Book is deleted, so Restrict on the Tag side.
        modelBuilder.Entity<BookTag>(bookTag =>
        {
            bookTag.HasKey(bt => new { bt.BookId, bt.TagId });

            bookTag.HasOne(bt => bt.Book)
                .WithMany(b => b.BookTags)
                .HasForeignKey(bt => bt.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            bookTag.HasOne(bt => bt.Tag)
                .WithMany(t => t.BookTags)
                .HasForeignKey(bt => bt.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
