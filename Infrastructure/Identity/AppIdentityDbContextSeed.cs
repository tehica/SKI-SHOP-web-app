using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "toni",
                    Email = "toni@test.com",
                    UserName = "toni@test.com",
                    Address = new Address
                    {
                        FirstName = "Toni",
                        LastName = "Name",
                        Street = "10 Street",
                        City = "Kuala Lumpur",
                        State = "Malaysia",
                        ZipCode = "10101"
                    }
                };

                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }
    }
}
