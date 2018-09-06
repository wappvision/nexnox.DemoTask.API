using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexnox.DemoTask.API.Data;
using System.Threading.Tasks;

namespace nexnox.DemoTask.API.Controllers
{
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        public AppDbContext _dbContext { get; set; }

        public PostsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize("Read")]
        public async Task<IActionResult> Get()
        {
            var posts = await _dbContext.Posts.ToListAsync();
            return Ok(posts);
        }
    }
}
