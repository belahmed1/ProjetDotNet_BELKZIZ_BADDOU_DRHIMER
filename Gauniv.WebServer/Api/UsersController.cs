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
            // Retrieve all users along with an "Online" flag based on OnlineHub's ConnectedUsers dictionary.
            var users = await _dbContext.Users
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FirstName,
                    u.LastName,
                    // Check if the user is in the ConnectedUsers dictionary.
                    Online = OnlineHub.ConnectedUsers.Values.Any(status => status.User != null && status.User.Id == u.Id)
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}
