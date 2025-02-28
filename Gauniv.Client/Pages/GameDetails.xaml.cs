using Gauniv.Client.ViewModel; 
using Microsoft.Maui.Controls;

namespace Gauniv.Client.Pages
{
    public partial class GameDetails : ContentPage
    {
        public GameDetails()
        {
            InitializeComponent();  
            BindingContext = new GameDetailsViewModel();
        }
    }
}
