using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gauniv.Client.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Gauniv.WebServer.Dtos;
using System.Collections.Generic;
using System.Linq;
using Gauniv.Client.Pages;

namespace Gauniv.Client.ViewModel
{
    public partial class MyGamesViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new ApiService();

        [ObservableProperty]
        private ObservableCollection<GameDto> ownedGames = new();

        [ObservableProperty]
        private ObservableCollection<string> availableCategories = new();

        [ObservableProperty]
        private string selectedCategory = "All";

        // Optional: Maximum price filter
        [ObservableProperty]
        private decimal maxPrice = 0;

        [ObservableProperty]
        private int offset = 0;

        [ObservableProperty]
        private int limit = 10;

        [ObservableProperty]
        private bool isLoading = false;

        public MyGamesViewModel()
        {
            LoadCategories();
            LoadOwnedGamesAsync().ConfigureAwait(false);
        }

        private void LoadCategories()
        {
            AvailableCategories.Clear();
            AvailableCategories.Add("All");
            AvailableCategories.Add("Action");
            AvailableCategories.Add("Adventure");
            AvailableCategories.Add("RPG");
            AvailableCategories.Add("Strategy");
            // Add more if needed.
        }

        partial void OnSelectedCategoryChanged(string value)
        {
            Offset = 0;
            OwnedGames.Clear();
            LoadOwnedGamesAsync().ConfigureAwait(false);
        }

        partial void OnMaxPriceChanged(decimal value)
        {
            Offset = 0;
            OwnedGames.Clear();
            LoadOwnedGamesAsync().ConfigureAwait(false);
        }

        [RelayCommand]
        private async Task LoadOwnedGamesAsync()
        {
            if (isLoading) return;
            isLoading = true;
            try
            {
                var list = await _apiService.GetOwnedGamesAsync(offset, limit);

                // Filter by category client-side
                if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "All")
                {
                    list = list.Where(g => g.Categories.Exists(c =>
                        c.Name.Equals(SelectedCategory, System.StringComparison.OrdinalIgnoreCase))).ToList();
                }

                // Filter by max price (if applicable)
                if (MaxPrice > 0)
                {
                    list = list.Where(g => g.Price <= MaxPrice).ToList();
                }

                if (offset == 0)
                    OwnedGames.Clear();

                foreach (var g in list)
                {
                    OwnedGames.Add(g);
                }
                offset += limit;
            }
            catch (System.Exception ex)
            {
                // Handle error
            }
            finally
            {
                isLoading = false;
            }
        }

        [RelayCommand]
        private void LoadMoreOwnedGames()
        {
            LoadOwnedGamesAsync().ConfigureAwait(false);
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
