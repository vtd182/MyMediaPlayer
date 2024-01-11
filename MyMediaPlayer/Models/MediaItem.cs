using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;

namespace MyMediaPlayer.Models;
public class MediaItem
{
    public string Name
    {
        get; set;
    }
    public string FilePath
    {
        get; set;
    }
    public BitmapImage Thumbnail
    {
        get; set;
    }

    public MediaItem(string name, string filePath, BitmapImage thumbnail = null)
    {
        Name = name;
        FilePath = filePath;
        Thumbnail = thumbnail;
    }
}
