using System;
using System.Collections.Generic;
using System.Linq;

namespace Saber3D.Common
{

  public static class LinqExtensions
  {

    public static IEnumerable<T> TakeLast<T>( this IEnumerable<T> collection, int count )
      => collection.Skip( Math.Max( 0, collection.Count() - count ) );

  }

}
