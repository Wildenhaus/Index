using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Index.UI.Converters
{

  [ValueConversion( typeof( Visibility ), typeof( bool ) )]
  public class BoolVisibilityConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      if ( targetType == typeof( Visibility ) )
      {
        if ( value is bool isVisible )
          return isVisible ? Visibility.Visible : Visibility.Hidden;
        else
          return Visibility.Hidden;
      }
      else if ( targetType == typeof( bool ) )
      {
        if ( value is Visibility visibility )
          return visibility == Visibility.Visible;
      }

      return null;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
      => Convert( value, targetType, parameter, culture );
  }

  [ValueConversion( typeof( Visibility ), typeof( int ) )]
  public class IntVisibilityConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      if ( targetType == typeof( Visibility ) )
      {
        if ( value is int isVisible )
          return isVisible > 0 ? Visibility.Visible : Visibility.Hidden;
        else
          return Visibility.Hidden;
      }
      else if ( targetType == typeof( int ) )
      {
        if ( value is Visibility visibility )
          return visibility == Visibility.Visible ? 1 : 0;
      }

      return null;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
      => Convert( value, targetType, parameter, culture );
  }

  [ValueConversion( typeof( Visibility ), typeof( ICollection ) )]
  public class CollectionVisibilityConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      if ( targetType == typeof( Visibility ) )
      {
        if ( value is ICollection collection )
          return collection.Count > 0 ? Visibility.Visible : Visibility.Hidden;
        else
          return Visibility.Hidden;
      }

      return null;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
      => Convert( value, targetType, parameter, culture );
  }

  [ValueConversion( typeof( Visibility ), typeof( object ) )]
  public class NullVisibilityConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      return value is null ? Visibility.Hidden : Visibility.Visible;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
      => Convert( value, targetType, parameter, culture );
  }

}
