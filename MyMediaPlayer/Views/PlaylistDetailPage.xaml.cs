using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MyMediaPlayer.Models;
using MyMediaPlayer.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyMediaPlayer.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PlaylistDetailPage : Page
{
    public HomePageViewModel ViewModel
    {
        get;
    }

    private HomePageViewModel viewModel = HomePageViewModel.Instance;
    public PlaylistDetailPage()
    {

        this.InitializeComponent();
        var data = viewModel.MediaItems;
        if (data != null)
        {
            this.MediaItemList.ItemsSource = data;

        }
    }

    private void RemoveItem_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;

        var itemToRemove = (MediaItem)button.DataContext;

        viewModel.RemoveItem(itemToRemove);
    }
}
