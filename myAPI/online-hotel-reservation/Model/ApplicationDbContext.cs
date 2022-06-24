using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace online_hotel_reservation.Model
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     base.OnConfiguring(optionsBuilder);
        //     optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=OrgAPIDb;Trusted_Connection=True;");
        // }

        public DbSet<AppUser> AppUsers { get ; set;}

        public DbSet<CustomerReservation> CustomerReservations { get ; set;}
    }
}

