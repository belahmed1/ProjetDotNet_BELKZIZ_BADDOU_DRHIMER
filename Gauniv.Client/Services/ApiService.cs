using System.Net.Http.Json;
using Gauniv.WebServer.Dtos;

namespace Gauniv.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = NetworkService.Instance.httpClient;
        }

        /// <summary>
        /// Retrieves all available games, with optional pagination and category filtering.
        /// </summary>
        public async Task<List<GameDto>> GetAllGamesAsync(int offset = 0, int limit = 10, string? category = null)
        {
            var url = $"https://localhost:7209/game?offset={offset}&limit={limit}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
            games ??= new List<GameDto>();

            // Client-side filtering if a valid category is provided and it's not "All"
            if (!string.IsNullOrEmpty(category))
            {
                category = category.Trim();
                if (!category.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    games = games.FindAll(g =>
                        g.Categories.Exists(c =>
                            c.Name.Equals(category, StringComparison.OrdinalIgnoreCase)));
                }
            }

            return games;
        }




        /// <summary>
        /// Retrieves the list of games owned by the connected user.
        /// </summary>
        public async Task<List<GameDto>> GetOwnedGamesAsync(int offset = 0, int limit = 10)
        {
            var url = $"https://localhost:7209/game/owned?offset={offset}&limit={limit}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
            return games ?? new List<GameDto>();
        }

        /// <summary>
        /// Retrieves the details of a game.
        /// </summary>
        public async Task<GameDto?> GetGameDetailsAsync(int gameId)
        {
            var url = $"https://localhost:7209/game/{gameId}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<GameDto>();
        }

        /// <summary>
        /// Retrieves categories from the database.
        /// Assumes an endpoint exists at /category that returns List<CategoryDto>.
        /// </summary>
        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var url = $"https://localhost:7209/category";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
            return categories ?? new List<CategoryDto>();
        }

        /// <summary>
        /// Authenticates a user using the provided credentials.
        /// Note: the API route is adjusted to match your server (api/1.0.0/Auth/Login).
        /// </summary>
        public async Task<string?> LoginAsync(string username, string password)
        {
            var url = "https://localhost:7209/api/1.0.0/Auth/Login";
            var credentials = new { Username = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync(url, credentials);
            response.EnsureSuccessStatusCode();

            // The server returns an object with a "token" property.
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return tokenResponse?.token;
        }
    }

    public class TokenResponse
    {
        // Make sure this matches what the server returns:
        public string token { get; set; }
    }
}
