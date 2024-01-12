using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MyMediaPlayer.Views;

public sealed partial class RenamePlaylistDialog : ContentDialog
{
    public string PlaylistName
    {
        get; private set;
    }

    public bool isCancel
    {
        get; private set; 
    }
    public RenamePlaylistDialog()
    {
        this.InitializeComponent();
    }

    private void Cancel_click(object sender, RoutedEventArgs e)
    {
        isCancel = true;
        this.Hide();

    }

    private void Rename_click(object sender, RoutedEventArgs e)
    {
        isCancel = false;
        PlaylistName = playlistNameTextBox.Text;
        this.Hide();
    }
}
