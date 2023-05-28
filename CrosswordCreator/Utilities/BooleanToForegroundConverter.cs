using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CrosswordCreator.Utilities
{
  class BooleanToForegroundConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool isTrue)
      {
        return isTrue ? Brushes.Black : Brushes.Transparent;
      }

      return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
