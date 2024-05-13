using MauiUI.ViewModel;

namespace MauiUI
{
    public partial class DetailPage : ContentPage
    {
        public DetailPage(DetailViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }

}