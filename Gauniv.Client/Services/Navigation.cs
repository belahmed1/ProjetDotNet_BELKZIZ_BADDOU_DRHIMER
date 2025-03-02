using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace Gauniv.Client.Services
{
    public partial class NavigationService : ObservableObject
    {
        public static NavigationService Instance { get; private set; } = new NavigationService();

        public bool CanGoBack => App.Current?.MainPage?.Navigation.NavigationStack.Count > 0;

        [ObservableProperty]
        private ContentPage currentPage;

        public async void GoBack() => await App.Current.MainPage.Navigation.PopAsync();

        /// <summary>
        /// Navigates to a page using Shell. Make sure the route is registered.
        /// </summary>
        public async void Navigate<T>(Dictionary<string, object> args, bool clear = false) where T : ContentPage
        {
            string route = typeof(T).Name;
            Routing.RegisterRoute(route, typeof(T));
            if (clear)
            {
                await Shell.Current.Navigation.PopToRootAsync();
            }
            System.Diagnostics.Debug.WriteLine($"Navigating to {route}");
            await Shell.Current.GoToAsync(route, args);
        }

    }
}
