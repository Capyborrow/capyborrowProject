using Microsoft.EntityFrameworkCore;
using capyborrowProject.Models;

namespace capyborrowProject.Data
{
    public class APIContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        public APIContext(DbContextOptions<APIContext> options) : base(options)
        {
        }
    }
}
