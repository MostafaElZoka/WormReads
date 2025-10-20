using Microsoft.EntityFrameworkCore;
using WormReads.Models;

namespace WormReads.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
    }
}
