﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace H2AIndex.UI.Converters
{

  public class StringNotEmptyOrNullConverter : IValueConverter
  {

    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      if ( value is string str )
        return !string.IsNullOrEmpty( str );

      return false;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }

  }

}
