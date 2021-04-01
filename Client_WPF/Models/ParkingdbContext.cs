using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Web_Service.Models;

namespace Client_WPF.Models
{
    public partial class ParkingdbContext : DbContext
    {
        public ParkingdbContext()
        {
        }

        public ParkingdbContext(DbContextOptions<ParkingdbContext> options) : base(options)
        {
        }

        public virtual DbSet<ParkingInfo> Parks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            }
        }

    }
}