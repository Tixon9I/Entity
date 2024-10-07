using ContextLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ContextLibrary
{
    public class LibraryContextFactory : IDesignTimeDbContextFactory<Context.LibraryContext>
    {
        public Context.LibraryContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Context.LibraryContext>();

            optionsBuilder.UseSqlServer("Data Source=DESKTOP-T9TKG8K\\SQLEXPRESS;Initial Catalog=Library;Integrated Security=True;Encrypt=False");

            return new Context.LibraryContext(optionsBuilder.Options);
        }
    }

    public class Context
    {
        public class LibraryContext : DbContext
        {
            public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

            public DbSet<Employee> Employees { get; set; }
            public DbSet<Visitor> Visitors { get; set; }
            public DbSet<Author> Authors { get; set; }
            public DbSet<Book> Books { get; set; }
            public DbSet<TypeFile> TypeFiles { get; set; }
            public DbSet<TypePublisherCode> TypePublisherCodes { get; set; }
            public DbSet<AuthorBook> AuthorBooks { get; set; }
            public DbSet<BorrowedBook> BorrowedBooks { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<AuthorBook>()
                    .HasKey(ab => new { ab.AuthorId, ab.BookId });

                modelBuilder.Entity<AuthorBook>()
                    .HasOne(ab => ab.Author)
                    .WithMany(a => a.AuthorBooks)
                    .HasForeignKey(ab => ab.AuthorId);

                modelBuilder.Entity<AuthorBook>()
                    .HasOne(ab => ab.Book)
                    .WithMany(b => b.AuthorBooks)
                    .HasForeignKey(ab => ab.BookId);

                modelBuilder.Entity<Book>()
                    .HasOne(b => b.Author)
                    .WithMany()
                    .HasForeignKey(b => b.AuthorId);

                modelBuilder.Entity<Employee>()
                    .HasIndex(e => e.Login)
                    .IsUnique();

                modelBuilder.Entity<Employee>()
                    .HasIndex(e => e.Password)
                    .IsUnique();

                modelBuilder.Entity<Author>()
                    .HasIndex(a => a.Pseudonym)
                    .IsUnique();

                modelBuilder.Entity<Visitor>()
                    .HasIndex(v => v.Login)
                    .IsUnique();

                modelBuilder.Entity<BorrowedBook>()
                    .HasOne(b => b.Visitor)
                    .WithMany()
                    .HasForeignKey(b => b.VisitorId)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<BorrowedBook>()
                    .HasOne(b => b.Book)
                    .WithMany()
                    .HasForeignKey(b => b.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
