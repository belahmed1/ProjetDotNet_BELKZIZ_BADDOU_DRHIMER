using CommunityToolkit.Mvvm.ComponentModel;
using Gauniv.Client.Services;
using Gauniv.WebServer.Dtos;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Gauniv.Client.ViewModel
{
    [QueryProperty(nameof(GameId), "GameId")]
    public partial class GameDetailsViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new ApiService();

        [ObservableProperty]
        private GameDto selectedGame;

        private int gameId;
        public int GameId
        {
            get => gameId;
            set
            {
                if (SetProperty(ref gameId, value))
                {
                    _ = LoadGameDetailsAsync(value);
                }
            }
        }

        private async Task LoadGameDetailsAsync(int id)
        {
            try
            {
                Debug.WriteLine($"LoadGameDetailsAsync called with gameId={id}");
                var game = await _apiService.GetGameDetailsAsync(id);
                if (game != null)
                {
                    SelectedGame = game;
                    Debug.WriteLine($"Game loaded: {game.Name}");
                }
                else
                {
                    Debug.WriteLine("No game returned from API.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadGameDetailsAsync error: {ex.Message}");
            }
        }
    }
}
