using Microsoft.Maui.Controls;
using Gauniv.Client.ViewModel;

namespace Gauniv.Client.Pages
{
    public partial class Index : ContentPage
    {
        public Index()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is IndexViewModel vm)
            {
                await vm.InitializeAsync();
            }
        }
    }
}
