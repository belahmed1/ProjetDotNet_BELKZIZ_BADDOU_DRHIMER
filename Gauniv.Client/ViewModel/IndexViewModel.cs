using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gauniv.Client.Pages;
using Gauniv.Client.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gauniv.WebServer.Dtos;

namespace Gauniv.Client.ViewModel
{
    public partial class IndexViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new ApiService();

        [ObservableProperty]
        private ObservableCollection<GameDto> games = new();

        [ObservableProperty]
        private int offset = 0;

        [ObservableProperty]
        private int limit = 10;

        public IndexViewModel()
        {
            // Optionnel : Charger dès le constructeur
            LoadGamesCommand.Execute(null);
            Games = new ObservableCollection<GameDto>
            {
                new GameDto { Name = "Game 1", Description = "Description 1" },
                new GameDto { Name = "Game 2", Description = "Description 2" }
            };
        }

        [RelayCommand]
        private async Task LoadGamesAsync()
        {
            try
            {
                var list = await _apiService.GetAllGamesAsync(Offset, Limit);
                Games.Clear();
                foreach (var g in list)
                {
                    Games.Add(g);
                }
            }
            catch (Exception ex)
            {
                // Gérer l’erreur (popup, log, etc.)
            }
        }

        // Pour naviguer vers les détails d’un jeu sélectionné
        [RelayCommand]
        private void GoToGameDetails(GameDto selectedGame)
        {
            if (selectedGame == null) return;

            // On peut passer l’ID du jeu comme paramètre
            var args = new Dictionary<string, object>
            {
                { "GameId", selectedGame.Id }
            };
            NavigationService.Instance.Navigate<GameDetails>(args);
        }
    }
}
