﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using LibVLCSharp.Shared;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyMediaPlayer.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CreatePlaylistDialog : ContentDialog
{
    public string PlaylistName
    {
        get; private set;
    }
    public CreatePlaylistDialog()
    {
        this.InitializeComponent();
    }

    private void ContentDialog_PrimaryButtonClick(object sender, RoutedEventArgs e)
    {
        PlaylistName = playlistNameTextBox.Text;
        Debug.WriteLine("Playlist name on dialog: " + PlaylistName);
        this.Hide();
    }
}
