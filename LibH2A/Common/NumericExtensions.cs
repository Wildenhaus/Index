using System.Diagnostics;

namespace Saber3D.Common
{

  public static class NumericExtensions
  {

    /// <summary>
    ///   Converts an SNorm-compressed 8-bit integer to a float.
    /// </summary>
    /// <param name="value">
    ///   The value to convert.
    /// </param>
    /// <returns>
    ///   The uncompressed value.
    /// </returns>
    [DebuggerHidden]
    public static float SNormToFloat( this sbyte value )
      => value / ( float ) sbyte.MaxValue; // TODO: Saber might handle this differently.

    /// <summary>
    ///   Converts an SNorm-compressed 16-bit integer to a float.
    /// </summary>
    /// <param name="value">
    ///   The value to convert.
    /// </param>
    /// <returns>
    ///   The uncompressed value.
    /// </returns>
    [DebuggerHidden]
    public static float SNormToFloat( this short value )
      => value / ( float ) short.MaxValue; // TODO: Saber might handle this differently.

    /// <summary>
    ///   Converts a UNorm-compressed 8-bit integer to a float.
    /// </summary>
    /// <param name="value">
    ///   The value to convert.
    /// </param>
    /// <returns>
    ///   The uncompressed value.
    /// </returns>
    [DebuggerHidden]
    public static float UNormToFloat( this byte value )
      => value / ( float ) byte.MaxValue; // TODO: Saber might handle this differently.

  }

}
