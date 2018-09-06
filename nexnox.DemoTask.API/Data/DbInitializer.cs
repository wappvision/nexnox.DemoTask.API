using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace nexnox.DemoTask.API.Data
{
    public class DbInitializer
    {
        public AppDbContext _dbContext { get; set; }
        public UserManager<IdentityUser> _userManager { get; set; }

        public DbInitializer(AppDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task Seed()
        {
            if (_dbContext.Database.EnsureCreated() == false || _dbContext.Users.Any() == true)
                return;
            
            for (int i = 1; i <= 3; i++)
            {
                await _userManager.CreateAsync(new IdentityUser()
                {
                    Email = $"user{i}@example.org",
                    UserName = $"user{i}",
                }, "secret");
            }

            var user1 = await _userManager.FindByEmailAsync("user1@example.org");
            await _userManager.AddClaimAsync(user1, new Claim("Permission", "ReadPost"));

            var user2 = await _userManager.FindByEmailAsync("user2@example.org");
            await _userManager.AddClaimAsync(user2, new Claim("Permission", "ReadPost"));
            await _userManager.AddClaimAsync(user2, new Claim("Permission", "WritePost"));

            await _dbContext.SaveChangesAsync();

            for (int i = 1; i <= 5; i++)
            {
                _dbContext.Posts.Add(new Models.Post()
                {
                    Title = $"Post #{i}",
                    Content = string.Join(" ", Enumerable.Repeat("Lorem Ipsum", i))
                });
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
