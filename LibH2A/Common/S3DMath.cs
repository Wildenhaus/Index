using System;
using System.Numerics;

namespace Saber3D.Common
{

  public static class S3DMath
  {

    /// <summary>
    ///   Returns the fractional part of each float in the vector.
    /// </summary>
    public static Vector2 Frac( in Vector2 value )
    {
      return new Vector2(
        x: value.X % 1.0f,
        y: value.Y % 1.0f
        );
    }

    /// <summary>
    ///   Returns the fractional part of each float in the vector.
    /// </summary>
    public static Vector3 Frac( in Vector3 value )
    {
      return new Vector3(
        x: value.X % 1.0f,
        y: value.Y % 1.0f,
        z: value.Z % 1.0f
        );
    }

    /// <summary>
    ///   Returns the value, normalized to a range of [0,1].
    /// </summary>
    public static float Saturate( in float value )
      => Math.Min( 1.0f, Math.Max( 0.0f, value ) );

    /// <summary>
    ///   Returns the sign of the input value.
    /// </summary>
    public static short Sign( in short value )
      => ( short ) ( value < 0 ? -1 : 1 );

  }

}
