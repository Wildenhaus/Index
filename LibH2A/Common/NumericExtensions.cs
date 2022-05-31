using System.Diagnostics;

namespace Saber3D.Common
{

  public static class NumericExtensions
  {

    /// <summary>
    ///   Converts a signed 16-bit integer to a float via SNorm.
    /// </summary>
    /// <param name="value">
    ///   The value to convert.
    /// </param>
    /// <returns>
    ///   The SNorm value.
    /// </returns>
    [DebuggerHidden]
    public static float ConvertToSNormFloat( this short value )
      => value / ( float ) short.MaxValue;

  }

}
