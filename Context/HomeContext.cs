using Microsoft.EntityFrameworkCore;

namespace LogReg.Models
{
    public class HomeContext : DbContext
    {
        public HomeContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}