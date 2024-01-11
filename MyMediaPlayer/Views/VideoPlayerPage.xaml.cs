using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MyMediaPlayer.ViewModels;

namespace MyMediaPlayer.Views;

public sealed partial class VideoPlayerPage : Page
{
    public VideoPlayerViewModel ViewModel
    {
        get;
    }

    public VideoPlayerPage()
    {
        ViewModel = App.GetService<VideoPlayerViewModel>();
        InitializeComponent();
    }
}
