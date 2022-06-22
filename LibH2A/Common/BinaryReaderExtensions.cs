using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using static Saber3D.Assertions;

namespace Saber3D.Common
{

  public static class BinaryReaderExtensions
  {

    /// <summary>
    ///   Peeks the next byte in the stream.
    /// </summary>
    /// <param name="reader">
    ///   The <see cref="BinaryReader" />.
    /// </param>
    /// <returns>
    ///   The next byte in the stream.
    /// </returns>
    [DebuggerHidden]
    public static byte PeekByte( this BinaryReader reader )
    {
      var value = reader.ReadByte();
      reader.BaseStream.Position -= 1;
      return value;
    }

    /// <summary>
    ///   Reads a <see cref="BitArray" /> from the stream.
    /// </summary>
    /// <param name="reader">
    ///   The <see cref="BinaryReader" />.
    /// </param>
    /// <param name="count">
    ///   The number of bits in the <see cref="BitArray" />.
    /// </param>
    /// <returns>
    ///   The read <see cref="BitArray"/>.
    /// </returns>
    [DebuggerHidden]
    public static BitArray ReadBitArray( this BinaryReader reader, int count )
    {
      var readLen = ( int ) Math.Ceiling( count / 8f );
      var buffer = new byte[ readLen ];
      reader.Read( buffer, 0, readLen );

      return new BitArray( buffer );
    }

    /// <summary>
    ///   Reads a <see cref="Matrix4x4" /> from the stream.
    /// </summary>
    /// <param name="reader">
    ///   The <see cref="BinaryReader" />.
    /// </param>
    /// <returns>
    ///   The read <see cref="Matrix4x4" />.
    /// </returns>
    [DebuggerHidden]
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

    /// <summary>
    ///   Reads a <see cref="Vector3" /> from the stream.
    /// </summary>
    /// <param name="reader">
    ///   The <see cref="BinaryReader" />.
    /// </param>
    /// <returns>
    ///   The read <see cref="Vector3" />
    /// </returns>
    [DebuggerHidden]
    public static Vector3 ReadVector3( this BinaryReader reader )
    {
      return new Vector3(
        x: reader.ReadSingle(),
        y: reader.ReadSingle(),
        z: reader.ReadSingle()
        );
    }

    /// <summary>
    ///   Reads a <see cref="Vector4" /> from the stream.
    /// </summary>
    /// <param name="reader">
    ///   The <see cref="BinaryReader" />.
    /// </param>
    /// <returns>
    ///   The read <see cref="Vector4" />.
    /// </returns>
    [DebuggerHidden]
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

    /// <summary>
    ///   Reads a null-terminated string.
    /// </summary>
    /// <param name="reader">
    ///   The <see cref="BinaryReader" />.
    /// </param>
    /// <returns>
    ///   The value of the string.
    /// </returns>
    [DebuggerHidden]
    public static string ReadStringNullTerminated( this BinaryReader reader )
    {
      var builder = new StringBuilder();

      var c = reader.ReadByte();
      while ( c != 0 )
      {
        builder.Append( ( char ) c );
        c = reader.ReadByte();
      }

      return builder.ToString();
    }

    /// <summary>
    ///   Reads a null-terminated string.
    /// </summary>
    /// <param name="reader">
    ///   The <see cref="BinaryReader" />.
    /// </param>
    /// <param name="maxLength">
    ///   The maximum length of the string.
    /// </param>
    /// <returns>
    ///   The value of the string.
    /// </returns>
    [DebuggerHidden]
    public static string ReadStringNullTerminated( this BinaryReader reader, int maxLength )
    {
      var builder = new StringBuilder();

      var c = reader.ReadByte();
      while ( c != 0 && builder.Length < maxLength )
      {
        builder.Append( ( char ) c );
        c = reader.ReadByte();
      }

      return builder.ToString();
    }

    /// <summary>
    ///   Reads a 128-bit <see cref="Guid" />.
    /// </summary>
    /// <param name="reader">
    ///   The <see cref="BinaryReader" />.
    /// </param>
    /// <returns>
    ///   The <see cref="Guid" />.
    /// </returns>
    [DebuggerHidden]
    public static Guid ReadGuid( this BinaryReader reader )
    {
      return new Guid(
        a: reader.ReadUInt32(),
        b: reader.ReadUInt16(),
        c: reader.ReadUInt16(),
        d: reader.ReadByte(),
        e: reader.ReadByte(),
        f: reader.ReadByte(),
        g: reader.ReadByte(),
        h: reader.ReadByte(),
        i: reader.ReadByte(),
        j: reader.ReadByte(),
        k: reader.ReadByte()
        );
    }

  }

}
