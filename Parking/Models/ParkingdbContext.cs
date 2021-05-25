using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Parking.Models
{
    public partial class ParkingdbContext : DbContext
    {
        public ParkingdbContext()
        {
            Database.EnsureCreated();
        }

        public ParkingdbContext(DbContextOptions<ParkingdbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<ParkingInfo> Parks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {          
                optionsBuilder.UseSqlite(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            }
        }

    }
}