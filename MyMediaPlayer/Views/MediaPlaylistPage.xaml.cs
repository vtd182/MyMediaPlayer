using Microsoft.UI.Xaml.Controls;

using MyMediaPlayer.ViewModels;

namespace MyMediaPlayer.Views;

public sealed partial class MediaPlaylistPage : Page
{
    public MediaPlaylistViewModel ViewModel
    {
        get;
    }

    public MediaPlaylistPage()
    {
        ViewModel = App.GetService<MediaPlaylistViewModel>();
        InitializeComponent();
    }
}
