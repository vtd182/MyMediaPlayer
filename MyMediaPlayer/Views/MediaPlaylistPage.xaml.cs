using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using MyMediaPlayer.ViewModels;
using MyMediaPlayer.Models;

using Windows.Foundation;
using System.Diagnostics;

namespace MyMediaPlayer.Views;

public sealed partial class MediaPlaylistPage : Page
{
    private static bool isLoaded = false;
    public MediaPlaylistViewModel ViewModel
    {
        get;
    }

    private MediaPlaylistViewModel viewModel = MediaPlaylistViewModel.Instance;

    public MediaPlaylistPage()
    {
        InitializeComponent();
        if (!isLoaded)
        {
            MediaPlaylistViewModel.Instance.Load();
            isLoaded = true;
        }
        var data = viewModel.Playlists;
        this.listView.ItemsSource = data;

    }

    private async void AddPlaylist_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Debug.WriteLine("AddPlaylist_Click is calling ");
        var dialog = new CreatePlaylistDialog();

        dialog.XamlRoot = this.Content.XamlRoot;
        var result = await dialog.ShowAsync();

        string playlistName = dialog.PlaylistName;
        if (viewModel.Playlists != null && !string.IsNullOrEmpty(playlistName))
        {
            Debug.WriteLine("Add playlist " + playlistName);
            viewModel.Playlists.Add(new MediaPlaylist(playlistName));
            viewModel.Save();
        } else
        {
            Debug.WriteLine("Nhập gì vào đi bạn ơi");
        }
    }

    private void ListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        MediaPlaylist selectedPlaylist = (MediaPlaylist)e.ClickedItem;
        if (selectedPlaylist != null)
        {
            int idx = viewModel.Playlists.IndexOf(selectedPlaylist);
            if (idx != -1)
            {
                // Đoạn này sẽ gửi cái playlist cho page tiếp theo để có thể nghe nhạc.
                Frame.Navigate(typeof(PlaylistDetailPage), idx);
            }
        }
    }

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;

        var itemToRemove = (MediaPlaylist)button.DataContext;
        viewModel.RemoveItem(itemToRemove);
    }
}
