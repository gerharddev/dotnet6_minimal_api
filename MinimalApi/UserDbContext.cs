using Microsoft.EntityFrameworkCore;

namespace MinimalApi
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions options) : base(options)
        {
        }

        protected UserDbContext()
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
