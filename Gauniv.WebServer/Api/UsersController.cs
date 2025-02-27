using Gauniv.WebServer.Data;
using Gauniv.WebServer.Websocket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Gauniv.WebServer.Api
{
    [Route("api/1.0.0/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public UsersController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/1.0.0/Users/List
        [HttpGet]
        public async Task<IActionResult> List()
        {
            // 1. Fetch users from the DB
            var users = await _dbContext.Users.ToListAsync();

            // 2. Do the "online check" in memory
            var result = users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.FirstName,
                u.LastName,
                Online = OnlineHub.ConnectedUsers.Values.Any(status => status.User?.Id == u.Id)
            })
            .ToList();

            // 3. Return the final result
            return Ok(result);
        }

    }
}
