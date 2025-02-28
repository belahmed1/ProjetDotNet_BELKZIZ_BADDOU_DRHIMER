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
            // Build the URL. If your API supports filtering by category, you can uncomment the following lines.
            var url = $"https://localhost:7209/game?offset={offset}&limit={limit}";
            // If your backend supports it, you might add:
            // if (!string.IsNullOrEmpty(category) && category != "All")
            // {
            //     url += $"&category={category}";
            // }

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
            games ??= new List<GameDto>();

            // Client-side filtering if a category is provided (and it's not "All")
            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                games = games.FindAll(g => g.Categories.Exists(c => c.Name.Equals(category, System.StringComparison.OrdinalIgnoreCase)));
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
    }
}
