using BookLibrarySystem.Data.Data;
using BookLibrarySystem.Data.Data.Configuration;
using BookLibrarySystem.Data.Models;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BookLibrarySystem.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public virtual DbSet<Book> Books { get; set; }  

        public DbSet<Author> Authors { get; set; }

        public DbSet<BookAuthor> BookAuthors { get; set; }

        public DbSet<BookLoan> Loans { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<WaitingList> WaitingList { get; set; }
        
        //public ApplicationDbContext() { }


        public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Book>().ToTable("Books");
            modelBuilder.Entity<Author>().ToTable("Authors");
            modelBuilder.Entity<BookLoan>().ToTable("Loans");
            modelBuilder.Entity<Reservation>().ToTable("Reservations");
            modelBuilder.Entity<WaitingList>().ToTable("WaitingList");

            modelBuilder.Entity<BookAuthor>().HasKey(a => new { a.AuthorId, a.BookId });
            modelBuilder.Entity<Book>().HasMany(x => x.Authors)
                .WithMany(x => x.Books)
                .UsingEntity<BookAuthor>();

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Loans);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Reservations);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.WaitingList);

            modelBuilder.Entity<Book>()
                .Property(p => p.Version)
                .IsRowVersion()
                .IsRequired(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(a => a.Loans);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(a => a.Reservations);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(a => a.WaitingList);

            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            //Seed database with data about books and authors
            new DbInitializer(modelBuilder).Seed();
        }
    }
}