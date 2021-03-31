using Microsoft.EntityFrameworkCore;

namespace Web_Service.Models
{
    public class ParkingdbMainContext : DbContext
    {
        public DbSet<ParkingInfo> Parks { get; set; }
        public ParkingdbMainContext(DbContextOptions<ParkingdbMainContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();           
        }
    }
}