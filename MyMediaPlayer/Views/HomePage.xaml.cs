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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage;
using MyMediaPlayer.Models;
using MyMediaPlayer.ViewModels;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyMediaPlayer.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class HomePage : Page
{
    private static bool isLoaded = false;
    public HomePageViewModel ViewModel
    {
        get;
    }

    private HomePageViewModel viewModel = HomePageViewModel.Instance;

    public HomePage()
    {
        Debug.WriteLine("init HomePage");
        this.InitializeComponent();

        if (!isLoaded)
        {
            HomePageViewModel.Instance.Load();
            isLoaded = true;
        }
        var data = viewModel.MediaItems;
        reRenderThumbnail(data);
        // Gán dữ liệu cho GridView
        MediaGridView.ItemsSource = data;
    }


    private async void reRenderThumbnail(ObservableCollection<MediaItem> list)
    {
        foreach(MediaItem item in list) {
            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(item.FilePath);
            if (storageFile.FileType == ".mp4")
            {
                // Nếu là video (.mp4), tạo Thumbnail từ video
                item.Thumbnail = await GenerateVideoThumbnail(storageFile);
            }
            else if (storageFile.FileType == ".mp3")
            {
                // Nếu là âm nhạc (.mp3), có thể thực hiện xử lý tương tự
                // Để đơn giản, bạn có thể để Thumbnail null hoặc tạo một hình ảnh nốt nhạc ở đây
            }

        }
    }
    // Các phương thức khác ở đây
    private bool IsFileExtensionMatch(string filePath, string extension)
    {
        string fileExtension = Path.GetExtension(filePath)?.ToLowerInvariant();

        // Kiểm tra xem phần mở rộng của tệp có phải là extension hay không
        return !string.IsNullOrEmpty(fileExtension) && fileExtension == $".{extension}";
    }
    private async void AddMediaButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
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
        if (viewModel.MediaItems != null)
        {
            if (!viewModel.MediaItems.Any(item => item.FilePath == newItem.FilePath))
            {
                // Thêm mới nếu chưa tồn tại
                viewModel.MediaItems.Add(newItem);

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

    private void MediaGridView_ItemClick(object sender, ItemClickEventArgs e)
    {
        Debug.WriteLine("seclectItem is calling");
        // Lấy MediaItem được chọn
        MediaItem selectedMedia = e.ClickedItem as MediaItem;

        if (selectedMedia != null)
        {
            Debug.WriteLine("seclectItem is not null");
            // Chuyển sang trang mới và truyền đối tượng MediaItem cho trang mới
            List<string> paths = new();
            paths.Add(selectedMedia.FilePath);
            Frame.Navigate(typeof(VideoPlayerPage), paths);
        }
        else
        {
            Debug.WriteLine("seclectItem is null");
        }
    }
}
