using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gauniv.Client.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Gauniv.WebServer.Dtos;
using System.Collections.Generic;
using Gauniv.Client.Pages;

namespace Gauniv.Client.ViewModel
{
    public partial class IndexViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new ApiService();

        // Collection of all loaded games
        [ObservableProperty]
        private ObservableCollection<GameDto> games = new();

        // List of categories to display in the dropdown
        [ObservableProperty]
        private ObservableCollection<string> availableCategories = new();

        // Currently selected category in the dropdown
        [ObservableProperty]
        private string selectedCategory = "All";

        [ObservableProperty]
        private int offset = 0;

        [ObservableProperty]
        private int limit = 10;

        [ObservableProperty]
        private bool isLoading = false;

        public IndexViewModel()
        {
            // Load categories (hard-coded for now)
            LoadCategories();
            // Load games initially
            LoadGamesAsync().ConfigureAwait(false);
        }

        // Hard-code some categories for demonstration
        private void LoadCategories()
        {
            AvailableCategories.Clear();
            AvailableCategories.Add("All");
            AvailableCategories.Add("Action");
            AvailableCategories.Add("Adventure");
            AvailableCategories.Add("RPG");
            AvailableCategories.Add("Strategy");
            AvailableCategories.Add("fantasy");
            // Add more categories as needed
        }

        // Called automatically when SelectedCategory changes (CommunityToolkit MVVM feature)
        partial void OnSelectedCategoryChanged(string value)
        {
            // Reset pagination and clear the list before reloading
            Offset = 0;
            Games.Clear();
            LoadGamesAsync().ConfigureAwait(false);
        }

        [RelayCommand]
        private async Task LoadGamesAsync()
        {
            if (isLoading) return;
            isLoading = true;

            try
            {
                // Pass the selected category to the API call
                var list = await _apiService.GetAllGamesAsync(offset, limit, selectedCategory);
                if (offset == 0)
                {
                    Games.Clear();
                }

                foreach (var g in list)
                {
                    Games.Add(g);
                }

                offset += limit; // Move pagination forward
            }
            catch (Exception ex)
            {
                // Log or display an error message as needed
            }
            finally
            {
                isLoading = false;
            }
        }

        [RelayCommand]
        private void LoadMoreGames()
        {
            LoadGamesAsync().ConfigureAwait(false);
        }

        [RelayCommand]
        private void GoToGameDetails(GameDto selectedGame)
        {
            if (selectedGame == null) return;

            var args = new Dictionary<string, object>
            {
                { "GameId", selectedGame.Id }
            };
            NavigationService.Instance.Navigate<GameDetails>(args);
        }
    }
}
