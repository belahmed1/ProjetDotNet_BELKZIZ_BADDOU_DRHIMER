using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using Gauniv.Client.Pages;
using Gauniv.Client.Services;

namespace Gauniv.Client.ViewModel
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new ApiService();

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [RelayCommand]
        public async Task LoginAsync()
        {
            Debug.WriteLine("Login command triggered.");
            try
            {
                var token = await _apiService.LoginAsync(Username, Password);
                Debug.WriteLine($"Received token: {token}");
                if (!string.IsNullOrEmpty(token))
                {
                    NetworkService.Instance.Token = token;
                    await App.Current.MainPage.DisplayAlert("Success", "Login successful", "OK");
                    NavigationService.Instance.Navigate<MyGames>(new Dictionary<string, object>());
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Login Error", "Invalid credentials.", "OK");
                    Debug.WriteLine("Login returned empty token.");
                }
            }
            catch (System.Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Login Exception", ex.Message, "OK");
                Debug.WriteLine($"LoginAsync error: {ex.Message}");
            }
        }
    }
}
