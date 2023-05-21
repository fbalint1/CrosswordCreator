using CrosswordCreator.Models.Enums;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace CrosswordCreator.Utilities
{
  internal class StatusEnumToDescriptionConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is StatusEnum statusEnum)
      {
        var descriptionAttribute = statusEnum.GetType().GetField(statusEnum.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

        return (descriptionAttribute.FirstOrDefault() as DescriptionAttribute)?.Description ?? string.Empty;
      }

      return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
