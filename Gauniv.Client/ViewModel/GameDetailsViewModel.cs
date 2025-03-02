using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gauniv.Client.Services;
using Gauniv.WebServer.Dtos;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.ViewModel
{
    [QueryProperty(nameof(GameId), "GameId")]
    public partial class GameDetailsViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new ApiService();

        [ObservableProperty]
        private GameDto selectedGame;

        [ObservableProperty]
        private bool isDownloaded;

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
                    // Initially mark the game as not downloaded.
                    IsDownloaded = false;
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

        [RelayCommand]
        public async Task DownloadGameAsync()
        {
            Debug.WriteLine("DownloadGameAsync command triggered.");
            if (SelectedGame == null)
            {
                Debug.WriteLine("DownloadGameAsync: SelectedGame is null.");
                return;
            }

            try
            {
                // Simulate a download operation (replace with real download logic)
                await Task.Delay(1000);
                IsDownloaded = true;
                await App.Current.MainPage.DisplayAlert("Téléchargé", "Le jeu a été téléchargé avec succès.", "OK");
                Debug.WriteLine("DownloadGameAsync: Game marked as downloaded.");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Erreur", ex.Message, "OK");
                Debug.WriteLine($"DownloadGameAsync error: {ex.Message}");
            }
        }

        [RelayCommand]
        public void PlayGame()
        {
            if (!IsDownloaded)
            {
                App.Current.MainPage.DisplayAlert("Info", "Le jeu n'est pas téléchargé.", "OK");
                return;
            }
            App.Current.MainPage.DisplayAlert("Jouer", "Lancement du jeu...", "OK");
        }

        [RelayCommand]
        public void DeleteGame()
        {
            if (!IsDownloaded)
            {
                App.Current.MainPage.DisplayAlert("Info", "Le jeu n'est pas téléchargé.", "OK");
                return;
            }
            IsDownloaded = false;
            App.Current.MainPage.DisplayAlert("Supprimé", "Le jeu a été supprimé.", "OK");
        }
    }
}
