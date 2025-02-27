using Gauniv.WebServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Gauniv.WebServer.Api
{
    [ApiController]
    [Route("api/1.0.0/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        #region Register Endpoint

        // DTO pour la requête de création d’utilisateur
        public class RegisterRequest
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        // POST: api/1.0.0/Auth/Register
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Vérifie si un utilisateur avec le même Username existe déjà
            var existingUser = await _userManager.FindByNameAsync(request.Username);
            if (existingUser != null)
                return BadRequest("Username already taken.");

            // Crée un nouvel utilisateur
            var user = new User
            {
                UserName = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                // Retourne les erreurs d'Identity (ex. mot de passe non conforme, etc.)
                return BadRequest(result.Errors);
            }

            return Ok("User created successfully.");
        }

        #endregion

        #region Login Endpoint

        // DTO pour la requête de login
        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        // POST: api/1.0.0/Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request.");

            // Recherche l'utilisateur par son Username
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                return Unauthorized("User does not exist.");

            // Vérifie le mot de passe
            var checkPassword = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!checkPassword)
                return Unauthorized("Incorrect password.");

            // Génère le token JWT
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        #endregion

        #region JWT Generation

        // Méthode interne pour générer le token JWT
        private string GenerateJwtToken(User user)
        {
            // Récupère la clé secrète depuis appsettings.json (section "Jwt:Secret")
            var secretKey = _configuration["Jwt:Secret"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Crée les claims (identifiant, nom d'utilisateur, etc.)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var token = new JwtSecurityToken(
                // Vous pouvez ajouter Issuer et Audience ici si nécessaire
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds,
                claims: claims
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}
