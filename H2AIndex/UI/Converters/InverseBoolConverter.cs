using System;
using System.Globalization;
using System.Windows.Data;

namespace H2AIndex.UI.Converters
{

  public class InverseBoolConverter : IValueConverter
  {

    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      return !value.Equals( true );
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }

  }

}
