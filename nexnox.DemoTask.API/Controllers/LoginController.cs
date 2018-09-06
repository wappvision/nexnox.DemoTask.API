using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexnox.DemoTask.API.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace nexnox.DemoTask.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        public UserManager<IdentityUser> _userManager { get; set; }
        public ITokenProvider<IdentityUser> _tokenProvider { get; set; }
        public AppDbContext _dbContext { get; set; }

        public LoginController(UserManager<IdentityUser> userManager, ITokenProvider<IdentityUser> tokenProvider, AppDbContext dbContext)
        {
            _userManager = userManager;
            _tokenProvider = tokenProvider;
            _dbContext = dbContext;
        }

        [HttpGet(nameof(Password))]
        [AllowAnonymous]
        public async Task<IActionResult> Password(string email, string password)
        {
            if (String.IsNullOrWhiteSpace(email) || String.IsNullOrWhiteSpace(password))
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(email.Trim().ToLower());
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.CheckPasswordAsync(user, password);

            if (result == false)
            {
                return NotFound();
            }

            var userPermissionClaims = await _dbContext.UserClaims
                .Where(x => x.ClaimType == "Permission" && x.UserId == user.Id)
                .Select(x => x.ToClaim())
                .ToListAsync();

            var token = _tokenProvider.CreateToken(user, DateTime.Now.AddYears(1), userPermissionClaims);

            return Ok(token);
        }
    }
}