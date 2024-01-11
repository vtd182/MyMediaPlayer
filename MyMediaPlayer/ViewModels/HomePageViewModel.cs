using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using MyMediaPlayer.Models;

namespace MyMediaPlayer.ViewModels;
public class HomePageViewModel : ObservableRecipient
{
    private static HomePageViewModel? instance;

    public ObservableCollection<MediaItem> MediaItems { get; set; } = new ObservableCollection<MediaItem>();

    // Private constructor để đảm bảo không thể tạo ra thêm thể hiện
    private HomePageViewModel()
    {
    }

    public static HomePageViewModel Instance
    {
        get
        {
            instance ??= new HomePageViewModel();
            return instance;
        }
    }

    public void Save()
    {
        Debug.WriteLine("SaveState method is called.");
        // Lưu trạng thái của MediaItems vào ApplicationData
        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        localSettings.Values["MediaItems"] = JsonConvert.SerializeObject(MediaItems);
    }

    public void Load()
    {
        Debug.WriteLine("LoadState method is called.");
        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        if (localSettings.Values.ContainsKey("MediaItems"))
        {
            var mediaItemsJson = localSettings.Values["MediaItems"]?.ToString();
            if (mediaItemsJson != null)
            {
                MediaItems = JsonConvert.DeserializeObject<ObservableCollection<MediaItem>>(mediaItemsJson);
                Debug.WriteLine(mediaItemsJson);
                Debug.WriteLine("Load");
                foreach (MediaItem item in  MediaItems)
                {
                    Debug.WriteLine(item.Name);
                }
            }
        }
    }
}
