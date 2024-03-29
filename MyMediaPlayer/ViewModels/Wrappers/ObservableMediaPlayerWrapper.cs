﻿using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using MyMediaPlayer.Models.Enums;
using DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace MyMediaPlayer.ViewModels.Wrappers;

public class ObservableMediaPlayerWrapper : ObservableObject
{
    private readonly MediaPlayer _player;
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly ILogger _log = Log.ForContext<ObservableMediaPlayerWrapper>();

    private int previousVolume;

    private const int rewindOffset10s = 10000;
    private const int rewindOffset3s = 3000;
    private const int rewindOffset60s = 60000;

    private const int volumeStep = 5;

    public ObservableMediaPlayerWrapper(MediaPlayer player, DispatcherQueue dispatcherQueue)
    {
        _player = player;
        _dispatcherQueue = dispatcherQueue;
        _player.TimeChanged += (sender, time) => _dispatcherQueue.TryEnqueue(() =>
        {
            OnPropertyChanged(nameof(TimeLong));
            OnPropertyChanged(nameof(TimeString));
        });

        _player.VolumeChanged += (sender, volumeArgs) => _dispatcherQueue.TryEnqueue(() =>
        {
            OnPropertyChanged(nameof(Volume));
        });

        _player.Playing += (sender, args) => _dispatcherQueue.TryEnqueue(() =>
        {
            OnPropertyChanged(nameof(IsPlaying));
        });

        _player.Paused += (sender, args) => _dispatcherQueue.TryEnqueue(() =>
        {
            OnPropertyChanged(nameof(IsPlaying));
        });

        _player.Stopped += (sender, args) => _dispatcherQueue.TryEnqueue(() =>
        {
            OnPropertyChanged(nameof(IsPlaying));
        });

        _player.EndReached += (sender, args) => _dispatcherQueue.TryEnqueue(() =>
        {
            OnPropertyChanged(nameof(IsPlaying));
        });

        _player.LengthChanged += (sender, args) => _dispatcherQueue.TryEnqueue(() =>
        {
            OnPropertyChanged(nameof(TotalTimeLong));
            OnPropertyChanged(nameof(TotalTimeString));
        });
    }

    public long TimeLong
    {
        get => _player.Time;
        set => SetProperty(_player.Time, value, _player, (u, n) => u.Time = n);
    }

    public string TimeString => TimeSpan.FromMilliseconds(TimeLong).ToString(@"hh\:mm\:ss");

    public long TotalTimeLong => _player.Length;

    public string TotalTimeString => TimeSpan.FromMilliseconds(TotalTimeLong).ToString(@"hh\:mm\:ss");

    public int Volume
    {
        get => _player.Volume;
        set => SetProperty(_player.Volume, value, _player, (u, n) => u.Volume = n);
    }

    public bool IsPlaying => _player.IsPlaying;

    public void VolumeUp()
    {
        if (Volume <= 200)
        {
            _log.Information("VolumeUp, old value {0}, new value {1}", Volume, Volume + volumeStep);
            Volume += volumeStep;
        }
    }

    public void VolumeDown()
    {
        if (Volume >= volumeStep)
        {
            _log.Information("VolumeDown, old value {0}, new value {1}", Volume, Volume - volumeStep);
            Volume -= volumeStep;
        }
    }

    public void Mute()
    {
        if (Volume == 0)
        {
            Volume = previousVolume;
            _log.Information("Unmute, old value {0}, new value {1}", 0, Volume);
        }
        else
        {
            previousVolume = Volume;
            Volume = 0;
            _log.Information("Mute, old value {0}, new value {1}", previousVolume, Volume);
        }
    }

    public void PlayPause()
    {
        if (!IsPlaying)
        {
            _player.Play();
            _log.Information("Play");
        }
        else
        {
            _player.Pause();
            _log.Information("Pause");
        }
    }

    public void Stop()
    {
        _player.Stop();
        _log.Information("Stop");
    }

    public void FastForward(RewindMode mode)
    {
        var offset = mode switch
        {
            RewindMode.Normal => rewindOffset10s,
            RewindMode.Short => rewindOffset3s,
            RewindMode.Long => rewindOffset60s,
            _ => rewindOffset10s,
        };

        TimeLong += offset;
        _log.Information("FastForward, offset {0} ms", offset);
    }

    public void Rewind(RewindMode mode)
    {
        var offset = mode switch
        {
            RewindMode.Normal => rewindOffset10s,
            RewindMode.Short => rewindOffset3s,
            RewindMode.Long => rewindOffset60s,
            _ => rewindOffset10s,
        };

        TimeLong -= offset;
        _log.Information("Rewind, offset {0} ms", offset);
    }
}