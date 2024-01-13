using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Platforms.Windows;
using LibVLCSharp.Shared;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Serilog;
using Windows.Storage;
using Windows.System;
using MyMediaPlayer.Contracts.Services;
using MyMediaPlayer.Contracts.ViewModels;
using MyMediaPlayer.Models.Enums;
using MyMediaPlayer.ViewModels.Wrappers;
using DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MyMediaPlayer.ViewModels;

public partial class VideoPlayerViewModel : ObservableRecipient, INavigationAware
{
    private MediaPlaylistViewModel mediaPlaylistViewModel = MediaPlaylistViewModel.Instance;
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly INavigationService _navigationService;
    private readonly IWindowPresenterService _windowPresenterService;
    private readonly ILogger _log;
    private int currentMediaIndex;
    private bool _isPlayAudio = false;

    public bool IsPlayAudio
    {
        get
        {
            return _isPlayAudio;
        }
        set
        {
            _isPlayAudio = value;
            OnPropertyChanged(nameof(IsPlayAudio));
            Debug.WriteLine("Change: " + _isPlayAudio);
        }
    }


    public int CurrentMediaIndex
    {
        get => currentMediaIndex;
        set => SetProperty(ref currentMediaIndex, value);
    }

    private ObservableCollection<string> playlistPaths = new ObservableCollection<string>();
    private ObservableCollection<string> originalPlaylistPaths = new();

    private bool isPlaylistVisible;
    public bool IsPlaylistVisible
    {
        get => isPlaylistVisible;
        set => SetProperty(ref isPlaylistVisible, value);
    }

    public ObservableCollection<string> PlaylistPaths
    {
        get => playlistPaths;
        set => SetProperty(ref playlistPaths, value);
    }

    private LibVLC libVLC;
    private MediaPlayer mediaPlayer;
    private string filePath = "";
    private ObservableMediaPlayerWrapper mediaPlayerWrapper;
    private Visibility controlsVisibility;

    private readonly DispatcherTimer controlsHideTimer = new()
    {
        Interval = TimeSpan.FromSeconds(1),
    };

    public VideoPlayerViewModel(INavigationService navigationService, IWindowPresenterService windowPresenterService, ILogger log)
    {
        _navigationService = navigationService;
        _windowPresenterService = windowPresenterService;
        _log = log;

        _windowPresenterService.WindowPresenterChanged += OnWindowPresenterChanged;

        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    }

    ~VideoPlayerViewModel()
    {
        Dispose();
    }

    private LibVLC LibVLC
    {
        get => libVLC;
        set => libVLC = value;
    }

    public MediaPlayer Player
    {
        get => mediaPlayer;
        set => SetProperty(ref mediaPlayer, value);
    }

    public string FilePath
    {
        get => filePath;
        set => SetProperty(ref filePath, value);
    }

    public ObservableMediaPlayerWrapper MediaPlayerWrapper
    {
        get => mediaPlayerWrapper;
        set => SetProperty(ref mediaPlayerWrapper, value);
    }

    public bool IsNotFullScreen => !_windowPresenterService.IsFullScreen;

    public Visibility ControlsVisibility
    {
        get => controlsVisibility;
        set => SetProperty(ref controlsVisibility, value);
    }

    public int RowSpan => _windowPresenterService.IsFullScreen ? 2 : 1;

    public bool LoadPlayer => FilePath != "Empty";

    //[RelayCommand]
    //private void VideoViewKeyDown(KeyRoutedEventArgs args)
    //{
    //    //switch (args.Key)
    //    //{
    //    //    case VirtualKey.Space:
    //    //        PlayPause();
    //    //        break;
    //    //    case VirtualKey.Escape:
    //    //        FullScreen();
    //    //        break;
    //    //    case VirtualKey.Up:
    //    //        VolumeUp();
    //    //        break;
    //    //    case VirtualKey.Down:
    //    //        VolumeDown();
    //    //        break;
    //    //    //case VirtualKey.Left:
    //    //    //    Rewind(new KeyboardAcceleratorInvokedEventArgs());
    //    //    //    break;
    //    //    //case VirtualKey.Right:
    //    //    //    FastForward(new KeyboardAcceleratorInvokedEventArgs());
    //    //    //    break;
    //    //    case VirtualKey.M:
    //    //        Mute();
    //    //        break;
    //    //    case VirtualKey.S:
    //    //        Stop();
    //    //        break;
    //    //    case VirtualKey.F:
    //    //        //FullScreen();
    //    //        break;
    //    //    case VirtualKey.P:
    //    //        PlayPause();
    //    //        break;
    //    //}
    //}

    [RelayCommand]
    private void Initialized(InitializedEventArgs eventArgs)
    {
        Debug.WriteLine("Init of viewmodel is calling.");

        //if (FilePath == "Empty")
        //{
        //    _log.Information("Skipping LibVLC initialization, because no media file specified.");
        //    return;
        //}

        //_log.Information("Initializing LibVLC");

        //LibVLC = new LibVLC(true, eventArgs.SwapChainOptions);
        //Player = new MediaPlayer(LibVLC);

        //var media = new Media(LibVLC, new Uri(FilePath));
        //Player.Play(media);
        //_log.Information("Starting playback of '{0}'", FilePath);

        //MediaPlayerWrapper = new ObservableMediaPlayerWrapper(Player, _dispatcherQueue);

        if (PlaylistPaths.Count == 0)
        {
            _log.Information("Skipping LibVLC initialization, because no media files specified.");
            Debug.WriteLine("Skipping LibVLC initialization, because no media files specified.");

            return;
        }


        _log.Information("Initializing LibVLC");

        LibVLC = new LibVLC(true, eventArgs.SwapChainOptions);
        Player = new MediaPlayer(LibVLC);

        //foreach (var path in PlaylistPaths)
        //{
        //    FilePath = path;
        //    var media = new Media(LibVLC, new Uri(path));
        //    Player.Play(media);
        //    _log.Information($"Starting playback of '{path}'");
        //    Debug.WriteLine($"Starting playback of '{path}'");
        //}



        FilePath = PlaylistPaths[currentMediaIndex];
        var media = new Media(LibVLC, new Uri(FilePath));

        string fileExtension = Path.GetExtension(FilePath)?.ToLower();
        if (fileExtension == ".mp3")
        {
            IsPlayAudio = true;
        }
        else
        {
            IsPlayAudio = false;
        }

        Player.Play(media);
        _log.Information($"Starting playback of '{FilePath}'");

        MediaPlayerWrapper = new ObservableMediaPlayerWrapper(Player, _dispatcherQueue);
    }


    [RelayCommand]
    private void PointerMoved(PointerRoutedEventArgs? args)
    {
        if (_windowPresenterService.IsFullScreen)
        {
            if (ControlsVisibility == Visibility.Collapsed)
            {
                ShowControls();
            }
            else
            {
                controlsHideTimer.Stop();
                controlsHideTimer.Start();
            }
        }
    }

    private void Timer_Tick(object? sender, object e)
    {
        HideControls();
        controlsHideTimer.Stop();
    }

    private void OnWindowPresenterChanged(object? sender, EventArgs e)
    {
        if (sender is not IWindowPresenterService windowPresenter)
        {
            return;
        }

        if (windowPresenter.IsFullScreen)
        {
            controlsHideTimer.Tick += Timer_Tick;
        }
        else
        {
            controlsHideTimer.Stop();
            controlsHideTimer.Tick -= Timer_Tick;
            ShowControls();
        }

        OnPropertyChanged(nameof(IsNotFullScreen));
        OnPropertyChanged(nameof(ControlsVisibility));
        OnPropertyChanged(nameof(RowSpan));
    }

    private void ShowControls()
    {
        ControlsVisibility = Visibility.Visible;
        _log.Information("Showing controls");
    }

    private void HideControls()
    {
        ControlsVisibility = Visibility.Collapsed;
        _log.Information("Hiding controls");
    }

    [RelayCommand]
    private void PlayPause()
    {
        MediaPlayerWrapper?.PlayPause();
    }

    [RelayCommand]
    private void Stop()
    {
        MediaPlayerWrapper?.Stop();
    }

    [RelayCommand]
    private void Mute()
    {
        MediaPlayerWrapper?.Mute();
    }

    [RelayCommand]
    private void FullScreen()
    {
        _windowPresenterService.ToggleFullScreen();
    }

    [RelayCommand]
    private void VolumeDown()
    {
        MediaPlayerWrapper?.VolumeDown();
    }

    [RelayCommand]
    private void VolumeUp()
    {
        MediaPlayerWrapper?.VolumeUp();
    }

    [RelayCommand]
    private void ScrollChanged(PointerRoutedEventArgs args)
    {
        var delta = args.GetCurrentPoint(null).Properties.MouseWheelDelta;
        if (delta > 0)
        {
            MediaPlayerWrapper?.VolumeUp();
        }
        else
        {
            MediaPlayerWrapper?.VolumeDown();
        }
        args.Handled = true;
    }

    [RelayCommand]
    private void FastForward(object args)
    {
        if (args is KeyboardAcceleratorInvokedEventArgs keyboardAcceleratorInvokedEventArgs)
        {
            var modifier = keyboardAcceleratorInvokedEventArgs.KeyboardAccelerator.Modifiers;
            switch (modifier)
            {
                case VirtualKeyModifiers.None:
                case VirtualKeyModifiers.Menu://10s
                    MediaPlayerWrapper?.FastForward(RewindMode.Normal);
                    break;
                case VirtualKeyModifiers.Control://60s
                    MediaPlayerWrapper?.FastForward(RewindMode.Long);
                    break;
                case VirtualKeyModifiers.Shift://3s
                    MediaPlayerWrapper?.FastForward(RewindMode.Short);
                    break;
            }
            keyboardAcceleratorInvokedEventArgs.Handled = true;
        }
        else
        {
            MediaPlayerWrapper?.FastForward(RewindMode.Normal);
        }
    }

    [RelayCommand]
    private void Rewind(object args)
    {
        if (args is KeyboardAcceleratorInvokedEventArgs keyboardAcceleratorInvokedEventArgs)
        {
            var modifier = keyboardAcceleratorInvokedEventArgs.KeyboardAccelerator.Modifiers;
            switch (modifier)
            {
                case VirtualKeyModifiers.None:
                case VirtualKeyModifiers.Menu://10s
                    MediaPlayerWrapper?.Rewind(RewindMode.Normal);
                    break;
                case VirtualKeyModifiers.Control://60s
                    MediaPlayerWrapper?.Rewind(RewindMode.Long);
                    break;
                case VirtualKeyModifiers.Shift://3s
                    MediaPlayerWrapper?.Rewind(RewindMode.Short);
                    break;
            }
            keyboardAcceleratorInvokedEventArgs.Handled = true;
        }
        else
        {
            MediaPlayerWrapper?.Rewind(RewindMode.Normal);
        }
    }

    [RelayCommand]
    public void Shuffle()
    {
        var random = new Random();
        var shuffledPlaylist = new ObservableCollection<string>(originalPlaylistPaths.OrderBy(x => random.Next()));

        PlaylistPaths = shuffledPlaylist;
        currentMediaIndex = 0;

        // Set the new media.
        FilePath = PlaylistPaths[currentMediaIndex];
        string fileExtension = Path.GetExtension(FilePath)?.ToLower();
        if (fileExtension == ".mp3")
        {
            IsPlayAudio = true;
        }
        else
        {
            IsPlayAudio = false;
        }
        var media = new Media(LibVLC, new Uri(FilePath));
        Player.Play(media);

        _log.Information("Shuffle, new index {0}", currentMediaIndex);
    }

    [RelayCommand]
    public void PlayNext()
    {
        if (PlaylistPaths.Count == 0)
        {
            _log.Information("PlaylistPaths is empty.");
            return;
        }

        // Increment the index.
        currentMediaIndex++;

        // If we're at the end of the list, wrap around to the start.
        if (currentMediaIndex >= PlaylistPaths.Count)
        {
            currentMediaIndex = 0;
        }

        // Set the new media.

        FilePath = PlaylistPaths[currentMediaIndex];
        string fileExtension = Path.GetExtension(FilePath)?.ToLower();
        if (fileExtension == ".mp3")
        {
            IsPlayAudio = true;
        }
        else
        {
            IsPlayAudio = false;
        }
        var media = new Media(LibVLC, new Uri(FilePath));
        Player.Play(media);

        _log.Information("PlayNext, new index {0}", currentMediaIndex);
    }

    [RelayCommand]
    public void PlayPrevious()
    {
        if (PlaylistPaths.Count == 0)
        {
            _log.Information("PlaylistPaths is empty.");
            return;
        }

        // Decrement the index.
        currentMediaIndex--;

        // If we're at the start of the list, wrap around to the end.
        if (currentMediaIndex < 0)
        {
            currentMediaIndex = PlaylistPaths.Count - 1;
        }

        // Set the new media.
        FilePath = PlaylistPaths[currentMediaIndex];

        var media = new Media(LibVLC, new Uri(FilePath));
        string fileExtension = Path.GetExtension(FilePath)?.ToLower();
        if (fileExtension == ".mp3")
        {
            IsPlayAudio = true;
        }
        else
        {
            IsPlayAudio = false;
        }
        Player.Play(media);

        _log.Information("PlayPrevious, new index {0}", currentMediaIndex);
    }

    [RelayCommand]
    public void TogglePlaylistVisibility()
    {
        IsPlaylistVisible = !IsPlaylistVisible;
    }
    public void OnNavigatedTo(object parameter)
    {
        //if (parameter is IReadOnlyList<IStorageItem> fileList)
        //{
        //    var filePath = fileList.First().Path;
        //    FilePath = filePath;
        //}

        Debug.WriteLine("OnNavigatedTo is calling from viewmodel. ");
        if (parameter is List<string> playlistPaths)
        {
            foreach (var path in playlistPaths)
            {
                PlaylistPaths.Add(path);
                originalPlaylistPaths.Add(path);
                Debug.WriteLine("Add success");
                Debug.WriteLine(path);
            }
        }
        else if (parameter is int idx)
        {
            var items = mediaPlaylistViewModel.Playlists[idx].Playlist;
            foreach (var item in items)
            {
                PlaylistPaths.Add(item.FilePath);
                originalPlaylistPaths.Add(item.FilePath);

            }
        }
    }

    public void OnNavigatedFrom()
    {
        try
        {
            Player.Stop();

        }
        catch
        {
            // eat
        }
        //Player.Playing -= Player_Playing;
        //Player.TimeChanged -= Player_TimeChanged;
        //Player.Media.DurationChanged -= Media_DurationChanged;
        //Player.MediaChanged -= Player_MediaChanged;
        //Player.Paused -= Player_Paused;
        //Player.Stopped -= Player_Stopped;
        //Player.VolumeChanged -= Player_VolumeChanged;
    }

    public void Dispose()
    {
        var mediaPlayer = Player;
        Player = null;
        mediaPlayer?.Dispose();
        LibVLC?.Dispose();
        LibVLC = null;
    }
}
