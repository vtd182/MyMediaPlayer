using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMediaPlayer.Models;
public class PlaylistManager
{
    private string _playlistName;
    private ObservableCollection<MediaItem> _mediaItems = new ObservableCollection<MediaItem>();
    private ObservableCollection<string> currentPlaylist;
    private int currentPlaylistIndex;

    public string PlaylistName
    {
        get
        {
        return _playlistName; 
        }
        set
        {
            if (_playlistName != value)
            {
                _playlistName = value;
            }
        }
    }
    public ObservableCollection<MediaItem> MediaItems
    {
        get => _mediaItems;
        set => _mediaItems = value;
    }

    public ObservableCollection<string> CurrentPlaylist
    {
        get => currentPlaylist;
        private set => currentPlaylist = value;
    }

    public int CurrentPlaylistIndex
    {
        get => currentPlaylistIndex;
        private set => currentPlaylistIndex = value;
    }

    public bool IsShuffleMode
    {
        get; set;
    }

    public PlaylistManager()
    {
        CurrentPlaylist = new ObservableCollection<string>();
        CurrentPlaylistIndex = 0;
        IsShuffleMode = false;
    }

    public PlaylistManager(string name)
    {
        _playlistName=name;
        CurrentPlaylist = new ObservableCollection<string>();
        CurrentPlaylistIndex = 0;
        IsShuffleMode = false;
    }

    public void AddToPlaylist(string filePath)
    {
        CurrentPlaylist.Add(filePath);
    }

    public void RemoveFromPlaylist(string filePath)
    {
        CurrentPlaylist.Remove(filePath);
    }

    public void ClearPlaylist()
    {
        CurrentPlaylist.Clear();
        CurrentPlaylistIndex = 0;
    }

    public void Next()
    {
        if (CurrentPlaylist.Count > 0)
        {
            if (IsShuffleMode)
            {
                Random random = new Random();
                CurrentPlaylistIndex = random.Next(0, CurrentPlaylist.Count);
            }
            else
            {
                CurrentPlaylistIndex = (CurrentPlaylistIndex + 1) % CurrentPlaylist.Count;
            }
        }
    }

    public void Previous()
    {
        if (CurrentPlaylist.Count > 0)
        {
            CurrentPlaylistIndex = (CurrentPlaylistIndex - 1 + CurrentPlaylist.Count) % CurrentPlaylist.Count;
        }
    }

    public string GetCurrentMediaPath()
    {
        if (CurrentPlaylist.Count > 0 && CurrentPlaylistIndex >= 0 && CurrentPlaylistIndex < CurrentPlaylist.Count)
        {
            return CurrentPlaylist[CurrentPlaylistIndex];
        }

        return null;
    }
}

