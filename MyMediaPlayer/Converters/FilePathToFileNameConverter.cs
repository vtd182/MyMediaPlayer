using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace MyMediaPlayer.Converters;
public class FilePathToFileNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string filePath)
        {
            // Lấy tên file từ đường dẫn
            return System.IO.Path.GetFileName(filePath);
        }

        // Trả về giá trị mặc định hoặc giá trị trống nếu không phải là đường dẫn hợp lệ
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        // Không cần implement chuyển ngược trong trường hợp này
        throw new NotImplementedException();
    }
}