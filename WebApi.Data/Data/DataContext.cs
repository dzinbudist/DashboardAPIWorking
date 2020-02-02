using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApi.Data.Entities;

namespace WebApi.Data.Data
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
        }

        public DbSet<User> Users { get; set; }
        public DbSet<LogModel> Logs { get; set; }
        public DbSet<DomainModel> Domains { get; set; }
    }
}