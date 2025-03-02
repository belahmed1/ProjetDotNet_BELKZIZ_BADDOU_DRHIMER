using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gauniv.Client.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Gauniv.WebServer.Dtos;
using System.Collections.Generic;
using Gauniv.Client.Pages;
using System;
using System.Diagnostics;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.ViewModel
{
    public partial class IndexViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new ApiService();

        [ObservableProperty]
        private ObservableCollection<GameDto> games = new();

        [ObservableProperty]
        private ObservableCollection<string> availableCategories = new();

        [ObservableProperty]
        private string selectedCategory = "All";

        [ObservableProperty]
        private int offset = 0;

        [ObservableProperty]
        private int limit = 10;

        // New property for maximum price filtering
        [ObservableProperty]
        private decimal maxPrice = 0;

        [ObservableProperty]
        private bool isLoading = false;

        /// <summary>
        /// Call this method from the page’s OnAppearing.
        /// </summary>
        public async Task InitializeAsync()
        {
            await LoadCategoriesAsync();
            Offset = 0;
            Games.Clear();
            await LoadGamesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var cats = await _apiService.GetCategoriesAsync();
                AvailableCategories.Clear();
                // Add default option
                AvailableCategories.Add("All");
                foreach (var cat in cats)
                {
                    if (!AvailableCategories.Contains(cat.Name))
                        AvailableCategories.Add(cat.Name);
                }
                Debug.WriteLine("Categories loaded.");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Category Error", ex.Message, "OK");
                Debug.WriteLine($"LoadCategoriesAsync error: {ex.Message}");
            }
        }

        // When SelectedCategory changes, reset the offset and reload games.
        partial void OnSelectedCategoryChanged(string value)
        {
            Offset = 0;
            Games.Clear();
            _ = LoadGamesAsync();
        }

        // When MaxPrice changes, you may also want to reload games.
        partial void OnMaxPriceChanged(decimal value)
        {
            Debug.WriteLine($"MaxPrice changed: {value}");
            Offset = 0;
            Games.Clear();
            _ = LoadGamesAsync();
        }


        [RelayCommand]
        public async Task LoadGamesAsync()
        {
            if (IsLoading) return;
            IsLoading = true;
            try
            {
                // Get games from API using offset and limit; filtering will be applied client‑side.
                var list = await _apiService.GetAllGamesAsync(Offset, Limit, SelectedCategory);

                // If a maximum price is set (greater than 0), filter the games by price.
                if (MaxPrice > 0)
                {
                    list = list.FindAll(g => g.Price <= MaxPrice);
                }

                if (Offset == 0)
                {
                    Games.Clear();
                }
                foreach (var g in list)
                {
                    Games.Add(g);
                }
                Debug.WriteLine($"Loaded {list.Count} games. Offset was {Offset}.");
                if (list.Count > 0)
                {
                    Offset += Limit;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Info", "No more games to load.", "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Load Games Error", ex.Message, "OK");
                Debug.WriteLine($"LoadGamesAsync error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void LoadMoreGames()
        {
            _ = LoadGamesAsync();
        }

        [RelayCommand]
        public void GoToGameDetails(GameDto selectedGame)
        {
            if (selectedGame == null) return;
            var args = new Dictionary<string, object>
            {
                { "GameId", selectedGame.Id }
            };
            NavigationService.Instance.Navigate<MyGamesDetails>(args);
        }

        // Command to navigate to the Login page.
        [RelayCommand]
        public void NavigateToLogin()
        {
            NavigationService.Instance.Navigate<LoginPage>(new Dictionary<string, object>());
        }
    }
}
