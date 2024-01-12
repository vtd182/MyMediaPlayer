using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyMediaPlayer.Models
{
    public class MediaPlaylist : INotifyPropertyChanged
    {
        private ObservableCollection<MediaItem> _playlist;
        private string _playlistName;

        public ObservableCollection<MediaItem> Playlist
        {
            get => _playlist;
            set
            {
                _playlist = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NumberOfSongs));
            }
        }


        public int NumberOfSongs => Playlist?.Count ?? 0;

        public string PlaylistName
        {
            get => _playlistName;
            set
            {
                _playlistName = value;
                OnPropertyChanged();
            }
        }

        public MediaPlaylist(string playlistName)
        {
            PlaylistName = playlistName;
            Playlist = new ObservableCollection<MediaItem>();
        }

        // Implement INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnCountChange()
        {
            OnPropertyChanged(nameof(NumberOfSongs));
        }
    }
}
