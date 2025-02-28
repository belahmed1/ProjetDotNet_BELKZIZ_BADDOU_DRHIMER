    using Gauniv.Client.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Gauniv.WebServer.Dtos;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;


namespace Gauniv.Client.ViewModel
    {
    public partial class ProfileViewModel : ObservableObject
    {
        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string email;

        [RelayCommand]
        public async Task LoadProfileAsync()
        {
            // Si vous avez un endpoint type /Auth/Me ou /User/Me
            // var user = await _apiService.GetCurrentUserAsync();
            // Username = user.UserName;
            // Email = user.Email;
        }

        [RelayCommand]
        public void SaveLocalSettings(string installFolder)
        {
            // Sauvegarde locale du dossier d’installation
        }
    }

}
