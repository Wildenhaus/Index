using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace H2AIndex.UI.Converters
{

  [ValueConversion( typeof( ICollection ), typeof( Visibility ) )]
  public class CollectionVisibilityConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      if ( value is ICollection collection )
        return collection.Count > 0 ? Visibility.Visible : Visibility.Hidden;
      else
        return Visibility.Hidden;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
      => Convert( value, targetType, parameter, culture );
  }

  [ValueConversion( typeof( Visibility ), typeof( int ) )]
  public class IntVisibilityConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      if ( value is int isVisible )
        return isVisible > 0 ? Visibility.Visible : Visibility.Hidden;
      else
        return Visibility.Hidden;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
      => Convert( value, targetType, parameter, culture );
  }

}
