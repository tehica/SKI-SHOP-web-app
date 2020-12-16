using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// Add-Migration IdentityInitial -p Infrastructure -s API -o Identity/Migrations -c AppIdentityDbContext
namespace Infrastructure.Identity
{
    // IdentityDbContext gives us the ability to query a session with our identity database
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
        {

        }


        // if we don't add this we get issues with identity and the primary key it's using for the ID
        // of the AppUser field
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
