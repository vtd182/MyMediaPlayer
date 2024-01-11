using Microsoft.UI.Xaml.Controls;

using MyMediaPlayer.ViewModels;

namespace MyMediaPlayer.Views;

public sealed partial class NewPlayListPage : Page
{
    public NewPlayListViewModel ViewModel
    {
        get;
    }

    public NewPlayListPage()
    {
        ViewModel = App.GetService<NewPlayListViewModel>();
        InitializeComponent();
    }
}
