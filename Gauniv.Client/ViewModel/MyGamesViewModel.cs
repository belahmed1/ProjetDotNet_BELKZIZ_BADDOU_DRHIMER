using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gauniv.Client.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Gauniv.WebServer.Dtos;

namespace Gauniv.Client.ViewModel
{
    public partial class MyGamesViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new ApiService();

        [ObservableProperty]
        private ObservableCollection<GameDto> ownedGames = new();

        [ObservableProperty]
        private int offset = 0;

        [ObservableProperty]
        private int limit = 10;

        [ObservableProperty]
        private string selectedCategory;

        [ObservableProperty]
        private bool showOnlyFreeGames = false;

        [RelayCommand]
        private async Task LoadOwnedGamesAsync()
        {
            var list = await _apiService.GetOwnedGamesAsync(offset, limit);

            if (offset == 0)
                OwnedGames.Clear();
            foreach (var g in list)
            {
                OwnedGames.Add(g);
            }
        }

        [RelayCommand]
        private void LoadMoreOwnedGames()
        {
            offset += limit;
            LoadOwnedGamesAsync().ConfigureAwait(false);
        }
    }
}
