using Gauniv.Client.ViewModel;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.Pages
{
    public partial class Index : ContentPage
    {
        public Index()
        {
            InitializeComponent();
            BindingContext = new IndexViewModel();
        }
    }
}
