using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gauniv.Client.Services;
using Gauniv.WebServer.Dtos;

namespace Gauniv.Client.ViewModel
{
    public partial class GameDetailsViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new ApiService();

        [ObservableProperty]
        private GameDto selectedGame;

        [ObservableProperty]
        private bool isDownloaded; // Gérer l’état local

        [RelayCommand]
        public async Task LoadGameDetailsAsync(int gameId)
        {
            try
            {
                var game = await _apiService.GetGameDetailsAsync(gameId);
                if (game != null)
                {
                    SelectedGame = game;
                    // Vérifier si déjà téléchargé
                    // isDownloaded = ... (vérification locale)
                }
            }
            catch (Exception ex)
            {
                // Gérer l’erreur
            }
        }

        [RelayCommand]
        public async Task DownloadGameAsync()
        {
            if (SelectedGame == null) return;
            // Appel à un endpoint de téléchargement ou
            // on télécharge le binaire via /game/download/{id}
            // Ensuite, isDownloaded = true
        }

        [RelayCommand]
        public void PlayGame()
        {
            if (!IsDownloaded) return;
            // Lancer un Process.Start(...) ou autre
        }

        [RelayCommand]
        public void DeleteGame()
        {
            // Supprimer le binaire local, isDownloaded = false
        }
    }
}
