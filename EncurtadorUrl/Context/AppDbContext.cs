using EncurtadorUrl.Models;
using Microsoft.EntityFrameworkCore;

namespace EncurtadorUrl.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<ShortenedUrl> Urls { get; set; }
    }
}
