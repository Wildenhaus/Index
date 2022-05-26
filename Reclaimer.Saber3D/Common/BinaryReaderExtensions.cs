using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using static Saber3D.Assertions;

namespace Saber3D.Common
{

  public static class BinaryReaderExtensions
  {

    public static byte PeekByte( this BinaryReader reader )
    {
      var value = reader.ReadByte();
      reader.BaseStream.Position -= 1;
      return value;
    }

    public static Matrix4x4 ReadMatrix4x4( this BinaryReader reader )
    {
      return new Matrix4x4(
        m11: reader.ReadSingle(),
        m12: reader.ReadSingle(),
        m13: reader.ReadSingle(),
        m14: reader.ReadSingle(),
        m21: reader.ReadSingle(),
        m22: reader.ReadSingle(),
        m23: reader.ReadSingle(),
        m24: reader.ReadSingle(),
        m31: reader.ReadSingle(),
        m32: reader.ReadSingle(),
        m33: reader.ReadSingle(),
        m34: reader.ReadSingle(),
        m41: reader.ReadSingle(),
        m42: reader.ReadSingle(),
        m43: reader.ReadSingle(),
        m44: reader.ReadSingle()
        );
    }

    public static Vector3 ReadVector3( this BinaryReader reader )
    {
      return new Vector3(
        x: reader.ReadSingle(),
        y: reader.ReadSingle(),
        z: reader.ReadSingle()
        );
    }

    public static Vector4 ReadVector4( this BinaryReader reader )
    {
      return new Vector4(
        x: reader.ReadSingle(),
        y: reader.ReadSingle(),
        z: reader.ReadSingle(),
        w: reader.ReadSingle()
        );
    }

    /// <summary>
    ///   Reads a pascal-style string.
    /// </summary>
    /// <param name="reader">
    ///   The <see cref="BinaryReader" />.
    /// </param>
    /// <param name="lengthDataTypeSize">
    ///   The bit-size of the data type that precedes the string and denotes it's length.
    /// </param>
    /// <returns>
    ///   The value of the pascal string.
    /// </returns>
    [DebuggerHidden]
    public static string ReadPascalString( this BinaryReader reader, uint lengthDataTypeSize )
    {
      uint stringLength;
      switch ( lengthDataTypeSize )
      {
        case 2:
          stringLength = reader.ReadUInt16();
          break;
        case 4:
          stringLength = reader.ReadUInt32();
          break;
        default:
          return FailReturn<string>( $"Unhandled pascal string length: {lengthDataTypeSize}" );
      }

      var builder = new StringBuilder();

      for ( var i = 0; i < stringLength; i++ )
        builder.Append( ( char ) reader.ReadByte() );

      return builder.ToString();
    }

    /// <summary>
    ///   Reads a pascal-style string with a 16-bit length specifier.
    /// </summary>
    /// <param name="reader">
    ///   The <see cref="BinaryReader" />.
    /// </param>
    /// <returns>
    ///   The value of the pascal string.
    /// </returns>
    [DebuggerHidden]
    public static string ReadPascalString16( this BinaryReader reader )
      => ReadPascalString( reader, sizeof( ushort ) );

    /// <summary>
    ///   Reads a pascal-style string with a 32-bit length specifier.
    /// </summary>
    /// <param name="reader">
    ///   The <see cref="BinaryReader" />.
    /// </param>
    /// <returns>
    ///   The value of the pascal string.
    /// </returns>
    [DebuggerHidden]
    public static string ReadPascalString32( this BinaryReader reader )
      => ReadPascalString( reader, sizeof( uint ) );

  }

}
