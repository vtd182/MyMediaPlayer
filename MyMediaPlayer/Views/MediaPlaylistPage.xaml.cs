using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using MyMediaPlayer.ViewModels;
using Windows.Foundation;

namespace MyMediaPlayer.Views;

public sealed partial class MediaPlaylistPage : Page
{
    public List<test> tests = new List<test>();

    public MediaPlaylistViewModel ViewModel
    {
        get;
    }

    public MediaPlaylistPage()
    {
        ViewModel = App.GetService<MediaPlaylistViewModel>();
        InitializeComponent();
        tests.Add(new test("Yêu thích", 10));
        tests.Add(new test("Sơn Tùng", 10));
        tests.Add(new test("Hà nội tao & mẹ mày", 10));
        this.listView.ItemsSource = tests;

    }

    public List<test> Playlists
    {
        get
        {
            return tests;
        }
    }

    private async void AddPlaylist_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var dialog = new CreatePlaylistDialog();

        dialog.XamlRoot = this.Content.XamlRoot;
        var result = await dialog.ShowAsync();

        // Xử lý kết quả 
        if (result == ContentDialogResult.Primary)
        {
            // Lấy tên từ cửa sổ và thêm playlist
            string playlistName = dialog.PlaylistName;
            // Thêm logic để xử lý tên 
        }
    }

    public class test
    {
        string _name;
        int _songCount;
        public test(string name, int count)
        {
            _name = name;
            _songCount = count;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
            _name = value; }
        }

        public int SongCount
        {
            get
            {
                return _songCount;
            }
            set
            {
            _songCount = value; }
        }
    }

    private void ListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        // Đoạn này sẽ gửi cái playlist cho page tiếp theo để có thể nghe nhạc.
        Frame.Navigate(typeof(PlaylistDetailPage));
    }
}
