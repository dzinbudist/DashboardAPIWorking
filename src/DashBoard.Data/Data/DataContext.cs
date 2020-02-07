using DashBoard.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DashBoard.Data.Data
{
    public class DataContext : DbContext
    {
        //protected readonly IConfiguration Configuration;

        public DataContext(/*IConfiguration configuration*/)
        {
            //Configuration = configuration;
        }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            //this constructor is needed for more simple UNIT testing.
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            //options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
        }

        public DbSet<User> Users { get; set; }
        public DbSet<LogModel> Logs { get; set; }
        public DbSet<DomainModel> Domains { get; set; }
    }
}