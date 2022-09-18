using Microsoft.EntityFrameworkCore;
using APPExpert_WebAPI.Entities;

namespace APPExpert_WebAPI.Helpers
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserMaster> UserMaster { get; set; }
    }

    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }
        public DbSet<UserMaster> UserMaster { get; set; }
    }
}