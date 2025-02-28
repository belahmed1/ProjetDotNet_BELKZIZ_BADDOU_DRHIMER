using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gauniv.Client.Services;
using System.Collections.ObjectModel;
using Gauniv.WebServer.Dtos;

namespace Gauniv.Client.ViewModel {
public partial class MyGamesViewModel : ObservableObject
{
    private readonly ApiService _apiService = new ApiService();

    [ObservableProperty]
    private ObservableCollection<GameDto> ownedGames = new();

    [RelayCommand]
    private async Task LoadOwnedGamesAsync()
    {
        var list = await _apiService.GetOwnedGamesAsync(0, 10);
        OwnedGames.Clear();
        foreach (var g in list)
        {
            OwnedGames.Add(g);
        }
    }
}

}



