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

            // Check if a user with the same username exists
            var existingUser = await _userManager.FindByNameAsync(request.Username);
            if (existingUser != null)
                return BadRequest("Username already taken.");

            // Create a new user
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
                return BadRequest(result.Errors);
            }

            // Assign the "User" role by default
            await _userManager.AddToRoleAsync(user, "User");

            return Ok("User created successfully.");
        }

        #endregion

        #region Login Endpoint

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        // POST: api/1.0.0/Auth/Login
        [HttpPost]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request.");

            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                return Unauthorized("User does not exist.");

            var checkPassword = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!checkPassword)
                return Unauthorized("Incorrect password.");

            // Generate JWT (including role claims)
            var token = await GenerateJwtTokenAsync(user);
            return Ok(new { token });
        }

        #endregion

        #region JWT Generation

        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            var secretKey = _configuration["Jwt:Secret"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT secret is not configured properly.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create claims (include user roles)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            // Retrieve roles from the database
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                // Optional: issuer, audience
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds,
                claims: claims
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}
