using CrosswordCreator.Models.Enums;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace CrosswordCreator.Utilities
{
  internal class StatusEnumToIconConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is StatusEnum statusEnum)
      {
        var descriptionAttribute = statusEnum.GetType().GetField(statusEnum.ToString()).GetCustomAttributes(typeof(IconAttribute), false);

        return (descriptionAttribute.FirstOrDefault() as IconAttribute)?.IconPath ?? string.Empty;
      }

      return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
