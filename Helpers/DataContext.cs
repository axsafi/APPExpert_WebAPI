using Microsoft.EntityFrameworkCore;
using APPExpert_WebAPI.Entities;

namespace APPExpert_WebAPI.Helpers
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}