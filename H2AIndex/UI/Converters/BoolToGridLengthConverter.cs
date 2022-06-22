using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace H2AIndex.UI.Converters
{

  public class BoolToGridLengthConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      var visible = ( bool ) value;
      if ( !( parameter is GridLength defaultLength ) || parameter is null )
        defaultLength = new GridLength( 1, GridUnitType.Auto );

      return visible ? defaultLength : new GridLength( 0 );
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }

}
