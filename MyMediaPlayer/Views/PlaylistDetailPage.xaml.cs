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
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using MyMediaPlayer.Models;
using MyMediaPlayer.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Vpn;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Eventing.Reader;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyMediaPlayer.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PlaylistDetailPage : Page
{
    string _playlistName = "Defaut";
    int _count = 0;
    public MediaPlaylistViewModel ViewModel
    {
        get;
    }

    private MediaPlaylistViewModel viewModel = MediaPlaylistViewModel.Instance;

    public ObservableCollection<MediaItem> MediaItems { get; set; } = new ObservableCollection<MediaItem>();

    int idx = -1;

    public string PlaylistName
    {
        get
        {
            return _playlistName;
        }

        set
        {
            _playlistName = value;
        }
    }

    public int Count
    {
        get
        {
            return _count;
        }

        set
        {
            _count = value;
        }
    }
    public PlaylistDetailPage()
    {
        Debug.WriteLine("init PlaylistDetailPage");

        this.InitializeComponent();
        //this.MediaItemList.ItemsSource = this.MediaItems;
    }

    private void RemoveItem_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;

        var itemToRemove = (MediaItem)button.DataContext;

        viewModel.Playlists[idx].Playlist.Remove(itemToRemove);
        viewModel.Playlists[idx].OnCountChange();
        viewModel.Save();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        Debug.WriteLine("OnNavigatedTo");
        base.OnNavigatedTo(e);

        if (e.Parameter != null)
        {
            idx = (int)e.Parameter;
            var data = viewModel.Playlists[idx];
            Debug.WriteLine("OnNavigateTo: " + viewModel.Playlists[idx].PlaylistName);
            
            Debug.WriteLine("OnNavigateTo - playlist: ");
            MediaItems = data.Playlist;
            foreach(MediaItem item in MediaItems)
            {
                Debug.WriteLine(item.Name);
            }
            this.MediaItemList.ItemsSource = data.Playlist;
            this.Grid_Display_playlist.DataContext = data;
            Count = data.NumberOfSongs;
            PlaylistName = data.PlaylistName;
        }
    }

    private async void RenameItem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new RenamePlaylistDialog();

        dialog.XamlRoot = this.Content.XamlRoot;
        var result = await dialog.ShowAsync();

        
        bool isCancel = dialog.isCancel;
        if (isCancel)
        {
            // ko lam j ca vi co lam j dau
        } else
        {
            string playlistName = dialog.PlaylistName;
            if (!string.IsNullOrEmpty(playlistName))
            {
                viewModel.Playlists[idx].PlaylistName = playlistName;
                viewModel.Save();
            }
            else
            {
                Debug.WriteLine("Nhập gì vào đi bạn ơi");
            }
        }
    }

    private async void AddItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var window = new Window();

        // Lấy handle của cửa sổ
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        var filePicker = new FileOpenPicker();
        filePicker.FileTypeFilter.Add(".mp3");
        filePicker.FileTypeFilter.Add(".mp4");

        // Sử dụng PickMultipleFilesAsync để chọn nhiều file
        WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);

        var selectedFiles = await filePicker.PickMultipleFilesAsync();

        if (selectedFiles != null && selectedFiles.Count > 0)
        {
            foreach (var selectedFile in selectedFiles)
            {
                BitmapImage thumbnail = null;

                if (selectedFile.FileType == ".mp4")
                {
                    // Nếu là video (.mp4), tạo Thumbnail từ video
                    thumbnail = await GenerateVideoThumbnail(selectedFile);
                }
                else if (selectedFile.FileType == ".mp3")
                {
                    // Nếu là âm nhạc (.mp3), có thể thực hiện xử lý tương tự
                    // Để đơn giản, bạn có thể để Thumbnail null hoặc tạo một hình ảnh nốt nhạc ở đây
                }

                MediaItem newMediaItem = new MediaItem(selectedFile.Name, selectedFile.Path, thumbnail);
                AddMediaItem(newMediaItem);
            }
        }
    }

    public void AddMediaItem(MediaItem newItem)
    {
        // Kiểm tra nếu MediaItem đã tồn tại
        if (viewModel.Playlists[idx] != null)
        {
            if (!viewModel.Playlists[idx].Playlist.Any(item => item.FilePath == newItem.FilePath))
            {
                // Thêm mới nếu chưa tồn tại
                viewModel.Playlists[idx].Playlist.Add(newItem);
                viewModel.Playlists[idx].OnCountChange();

                // Lưu trạng thái sau khi thêm mới
                viewModel.Save();
            }
        }

    }
    private async Task<BitmapImage> GenerateVideoThumbnail(StorageFile videoFile)
    {
        const uint requestedSize = 100; // Kích thước mong muốn cho hình thu nhỏ

        StorageItemThumbnail thumbnail = await videoFile.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.VideosView, requestedSize);

        var bitmapImage = new BitmapImage();
        bitmapImage.SetSource(thumbnail);
        return bitmapImage;
    }

    private void PlayAll_Click(object sender, RoutedEventArgs e)
    {
        if (idx != -1)
        {
            // Đoạn này sẽ gửi cái playlist cho page tiếp theo để có thể nghe nhạc.
            Frame.Navigate(typeof(VideoPlayerPage), idx);
        }
    }
}
