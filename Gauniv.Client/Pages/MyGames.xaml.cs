using Gauniv.Client.ViewModel;

namespace Gauniv.Client.Pages;

public partial class MyGamesPage : ContentPage
{
    public MyGamesPage()
    {
        InitializeComponent();
        BindingContext = new MyGamesViewModel();
    }
}
