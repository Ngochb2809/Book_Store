using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Book_Store.Models;

namespace Book_Store.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Book_Store.Models.Book> Book { get; set; }
        public DbSet<Book_Store.Models.Order> Order { get; set; }
        public DbSet<Book_Store.Models.User> User { get; set; }
    }
}