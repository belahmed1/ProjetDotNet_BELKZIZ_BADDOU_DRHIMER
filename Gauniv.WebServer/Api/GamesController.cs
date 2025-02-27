using AutoMapper;
using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gauniv.WebServer.Api
{
    [Route("api/1.0.0/[controller]/[action]")]
    [ApiController]
    public class GamesController(ApplicationDbContext appDbContext, IMapper mapper, UserManager<User> userManager) : ControllerBase
    {
        // Private fields
        private readonly ApplicationDbContext _appDbContext = appDbContext;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<User> _userManager = userManager;

        // GET: api/1.0.0/Games/List
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int offset = 0, [FromQuery] int limit = 10, [FromQuery] int? category = null)
        {
            var query = _appDbContext.Games.Include(g => g.Categories).AsQueryable();

            if (category.HasValue)
            {
                query = query.Where(g => g.Categories.Any(c => c.Id == category.Value));
            }

            var games = await query.Skip(offset).Take(limit).ToListAsync();
            var gameDtos = _mapper.Map<List<GameDto>>(games);
            return Ok(gameDtos);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyGames()
        {
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.Users
                .Include(u => u.PurchasedGames)
                    .ThenInclude(g => g.Categories)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Unauthorized();
            }

            var gameDtos = _mapper.Map<List<GameDto>>(user.PurchasedGames);
            return Ok(gameDtos);
        }

        [Authorize]
        [HttpPost("{gameId}")]
        public async Task<IActionResult> Purchase(int gameId)
        {
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.Users
                .Include(u => u.PurchasedGames)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Unauthorized();
            }

            var game = await _appDbContext.Games
                .Include(g => g.PurchasedBy)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
            {
                return NotFound("Game not found.");
            }

            if (!user.PurchasedGames.Any(g => g.Id == gameId))
            {
                user.PurchasedGames.Add(game);
                game.PurchasedBy.Add(user);
                await _appDbContext.SaveChangesAsync();
            }

            return NoContent();
        }

        // GET: api/1.0.0/Games/Get/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var game = await _appDbContext.Games
                .Include(g => g.Categories)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            var gameDto = _mapper.Map<GameDto>(game);
            return Ok(gameDto);
        }

        // GET: api/1.0.0/Games/Download/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Download(int id)
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

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var contentType = "application/octet-stream";
            var fileName = System.IO.Path.GetFileName(filePath);

            return File(stream, contentType, fileName);
        }

        // POST:     (Admin only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] GameDto dto)
        {
            // On crée l'entité Game, mais on gère manuellement les catégories
            var game = new Game
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                PayloadPath = dto.PayloadPath,
                // on initialise la liste de catégories plus bas
            };

            // Liste de catégories à associer
            var categories = new List<Category>();

            foreach (var catDto in dto.Categories)
            {
                if (catDto.Id > 0)
                {
                    // On cherche la catégorie existante
                    var existingCat = await _appDbContext.Categories.FindAsync(catDto.Id);
                    if (existingCat == null)
                    {
                        // L'ID est > 0 mais la catégorie n'existe pas : conflit ou erreur
                        return BadRequest($"La catégorie {catDto.Id} n'existe pas en base.");
                    }

                    // On réutilise la catégorie existante
                    categories.Add(existingCat);
                }
                else
                {
                    // Id == 0 => on considère que c'est une nouvelle catégorie
                    var newCat = new Category
                    {
                        Name = catDto.Name
                    };
                    // EFCore générera un Id auto-incrémenté
                    _appDbContext.Categories.Add(newCat);
                    categories.Add(newCat);
                }
            }

            // On associe la liste de catégories au Game
            game.Categories = categories;

            // On enregistre le jeu
            _appDbContext.Games.Add(game);
            await _appDbContext.SaveChangesAsync();

            // Optionnel : on peut mapper le résultat en GameDto pour le retour
            var resultDto = new GameDto
            {
                Id = game.Id,
                Name = game.Name,
                Description = game.Description,
                Price = game.Price,
                PayloadPath = game.PayloadPath,
                Categories = game.Categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };

            return CreatedAtAction(nameof(Get), new { id = game.Id }, resultDto);
        }



        // DELETE: api/1.0.0/Games/Delete/5 (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var game = await _appDbContext.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            _appDbContext.Games.Remove(game);
            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
