using AutoMapper;
using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Api
{
    [ApiController]
    public class PublicGamesController : ControllerBase
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public PublicGamesController(ApplicationDbContext appDbContext, IMapper mapper, UserManager<User> userManager)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _userManager = userManager;
        }

        // 1. Récupérer le binaire d’un jeu et le copier localement
        // GET: /game/download/{id}
        [HttpGet]
        [AllowAnonymous]
        [Route("game/download/{id}")]
        public async Task<IActionResult> DownloadGame(int id)
        {
            var game = await _appDbContext.Games.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                return NotFound("Game not found.");
            }

            var filePath = game.PayloadPath;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            // The FileStream is returned so that the file is streamed directly to the client.
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var contentType = "application/octet-stream";
            var fileName = System.IO.Path.GetFileName(filePath);
            return File(stream, contentType, fileName);
        }

        // 2. Lister les jeux accessibles (tout le monde)
        // GET: /game
        // Examples:
        // /game?offset=10&limit=15
        // /game?category=3
        // /game?category[]=3&category[]=4
        // /game?offset=10&limit=15&category[]=3&category[]=2
        [HttpGet]
        [AllowAnonymous]
        [Route("game")]
        public async Task<IActionResult> ListGames([FromQuery] int offset = 0, [FromQuery] int limit = 10, [FromQuery] int[]? category = null)
        {
            var query = _appDbContext.Games.Include(g => g.Categories).AsQueryable();

            if (category != null && category.Any())
            {
                query = query.Where(g => g.Categories.Any(c => category.Contains(c.Id)));
            }

            var games = await query.Skip(offset).Take(limit).ToListAsync();
            var gameDtos = _mapper.Map<List<GameDto>>(games);
            return Ok(gameDtos);
        }

        // 3. Lister les jeux possédés par le joueur connecté (avec filtre + pagination)
        // GET: /game/owned
        [HttpGet]
        [Authorize]
        [Route("game/owned")]
        public async Task<IActionResult> ListOwnedGames([FromQuery] int offset = 0, [FromQuery] int limit = 10, [FromQuery] int[]? category = null)
        {
            // Get the current user's ID
            var userId = _userManager.GetUserId(User);
            // Retrieve the user with their purchased games (including categories)
            var user = await _userManager.Users
                .Include(u => u.PurchasedGames)
                    .ThenInclude(g => g.Categories)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Unauthorized();
            }

            // Filter the user's purchased games in memory
            var ownedGames = user.PurchasedGames.AsQueryable();
            if (category != null && category.Any())
            {
                ownedGames = ownedGames.Where(g => g.Categories.Any(c => category.Contains(c.Id)));
            }

            var filteredGames = ownedGames.Skip(offset).Take(limit).ToList();
            var gameDtos = _mapper.Map<List<GameDto>>(filteredGames);
            return Ok(gameDtos);
        }

        // 4. Lister les catégories disponibles (tout le monde)
        // GET: /category
        [HttpGet]
        [AllowAnonymous]
        [Route("category")]
        public async Task<IActionResult> ListCategories()
        {
            var categories = await _appDbContext.Categories.ToListAsync();
            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            return Ok(categoryDtos);
        }
    }
}
