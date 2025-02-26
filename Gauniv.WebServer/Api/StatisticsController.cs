using Gauniv.WebServer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Api
{
    [Route("api/1.0.0/[controller]/[action]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public StatisticsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/1.0.0/Statistics/General
        [HttpGet]
        public async Task<IActionResult> General()
        {
            // Total number of games available.
            var totalGames = await _dbContext.Games.CountAsync();

            // Games per category (genre).
            var gamesPerCategory = await _dbContext.Categories
                .Select(c => new {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    GameCount = c.Games.Count
                })
                .ToListAsync();

            // Total number of users.
            var totalUsers = await _dbContext.Users.CountAsync();
            // Total number of purchased games across all users.
            var totalPurchasedGames = await _dbContext.Users
                .SelectMany(u => u.PurchasedGames)
                .CountAsync();
            // Average number of purchased games per account.
            var avgGamesPerAccount = totalUsers > 0 ? (double)totalPurchasedGames / totalUsers : 0.0;

            // Placeholders for advanced statistics:
            // You might later store actual playtime and concurrency data.
            double avgPlayTimePerGame = 0.0; // Average playtime (in minutes or hours) per game.
            int maxConcurrentPlayersOverall = 0; // Maximum number of players concurrently online.
            var maxConcurrentPlayersPerGame = new List<object>(); // For each game, you could list max concurrent players.

            return Ok(new
            {
                totalGames,
                gamesPerCategory,
                avgGamesPerAccount,
                avgPlayTimePerGame,
                maxConcurrentPlayersOverall,
                maxConcurrentPlayersPerGame
            });
        }
    }
}
