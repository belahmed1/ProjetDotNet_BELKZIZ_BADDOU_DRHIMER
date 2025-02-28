using System.Net.Http.Json;
using System.Text.Json;
using Gauniv.Client.Services;
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
        /// Récupère la liste de tous les jeux disponibles, avec pagination et filtres éventuels
        /// </summary>
        public async Task<List<GameDto>> GetAllGamesAsync(int offset = 0, int limit = 10)
        {
            // Ajustez l’URL selon votre API. Par exemple : https://localhost:7209/game?offset=0&limit=10
            var url = $"https://localhost:7209/game?offset={offset}&limit={limit}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // Suppose que l’API renvoie une liste JSON de GameDto
            var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
            return games ?? new List<GameDto>();
        }

        /// <summary>
        /// Récupère la liste des jeux possédés par l’utilisateur connecté
        /// </summary>
        public async Task<List<GameDto>> GetOwnedGamesAsync(int offset = 0, int limit = 10)
        {
            // Endpoint : /game/owned
            var url = $"https://localhost:7209/game/owned?offset={offset}&limit={limit}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
            return games ?? new List<GameDto>();
        }

        /// <summary>
        /// Récupère le détail d’un jeu
        /// </summary>
        public async Task<GameDto?> GetGameDetailsAsync(int gameId)
        {
            var url = $"https://localhost:7209/game/{gameId}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<GameDto>();
        }

        // Vous pourrez rajouter d’autres méthodes : DownloadGame, DeleteGame, etc.
    }
}
