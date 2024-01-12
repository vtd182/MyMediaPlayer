using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using MyMediaPlayer.Models;
using Newtonsoft.Json;

namespace MyMediaPlayer.ViewModels;

public partial class MediaPlaylistViewModel : ObservableRecipient
{
    // Chứa tất cả playlist, cần singleton để thống nhất một thể hiện cho nó
    private static MediaPlaylistViewModel? instance;

    public ObservableCollection<MediaPlaylist> Playlists { get; set; } = new ObservableCollection<MediaPlaylist>();


    public MediaPlaylistViewModel()
    {

    }

    public static MediaPlaylistViewModel Instance
    {
        get
        {
            instance ??= new MediaPlaylistViewModel();
            return instance;
        }
    }

    public void Save()
    {
        Debug.WriteLine("SaveState of playlists method is called.");
        // Lưu trạng thái của MediaItems vào ApplicationData
        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        localSettings.Values["Playlists"] = JsonConvert.SerializeObject(Playlists);
    }

    public void Load()
    {
        Debug.WriteLine("LoadState method of playlists is called.");
        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        if (localSettings.Values.ContainsKey("Playlists"))
        {
            var mediaItemsJson = localSettings.Values["Playlists"]?.ToString();
            if (mediaItemsJson != null)
            {
                Playlists = JsonConvert.DeserializeObject<ObservableCollection<MediaPlaylist>>(mediaItemsJson);
                Debug.WriteLine(mediaItemsJson);
                Debug.WriteLine("Load");
                foreach (MediaPlaylist item in Playlists)
                {
                    Debug.WriteLine(item.PlaylistName);
                }
            }
        }
    }

    public void RemoveItem(MediaPlaylist item)
    {
        Playlists.Remove(item);

        // Lưu trạng thái sau khi xóa
        Save();
    }
}
