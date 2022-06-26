using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Media3D;

namespace Index.UI.Converters
{

  [ValueConversion( typeof( Point3D ), typeof( string ) )]
  public class Point3DConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      if ( value is Point3D point3D )
        return $"<{point3D.X:0.000},{point3D.Y:0.000},{point3D.Z:0.000}>";

      return value.ToString();
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }

  [ValueConversion( typeof( Vector3D ), typeof( string ) )]
  public class Vector3DConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      if ( value is Vector3D vector3D )
        return $"<{vector3D.X:0.000},{vector3D.Y:0.000},{vector3D.Z:0.000}>";

      return value.ToString();
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }

}
