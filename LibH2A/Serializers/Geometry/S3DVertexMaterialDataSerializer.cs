using System;
using System.IO;
using System.Linq;
using System.Numerics;
using Saber3D.Common;
using Saber3D.Data;
using Saber3D.Data.Geometry;
using static Saber3D.Assertions;

namespace Saber3D.Serializers.Geometry
{

  public class S3DInterleavedDataSerializer
  {

    #region Properties

    private S3DGeometryBuffer Buffer { get; }

    private S3DGeometryBufferFlags Flags
    {
      get => Buffer.Flags;
    }

    #endregion

    #region Constructor

    public S3DInterleavedDataSerializer( S3DGeometryBuffer buffer )
    {
      Buffer = buffer;
    }

    #endregion

    #region Public Methods

    public S3DInterleavedData Deserialize( BinaryReader reader )
    {
      var startPos = reader.BaseStream.Position;
      var endPos = reader.BaseStream.Position + Buffer.ElementSize;

      var dataArr = new byte[ Buffer.ElementSize ];
      reader.Read( dataArr, 0, Buffer.ElementSize );
      reader.BaseStream.Position -= Buffer.ElementSize;
      var dataStr = string.Join( " ", dataArr.Select( x => x.ToString( "X2" ) ) );

      var data = new S3DInterleavedData();

      //if ( Buffer.ElementSize == 0xc )
      //  System.Diagnostics.Debugger.Break();

      ReadTangents( reader, data );
      ReadColors( reader, data );
      ReadUVs( reader, data );

      var readSize = reader.BaseStream.Position - startPos;
      Assert( Buffer.ElementSize == readSize );
      return data;
    }

    #endregion

    #region Private Methods

    private void ReadTangents( BinaryReader reader, S3DInterleavedData data )
    {
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TANG0 ) )
        data.Tangent0 = ReadTangent( reader );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TANG1 ) )
        data.Tangent1 = ReadTangent( reader );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TANG2 ) )
        data.Tangent2 = ReadTangent( reader );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TANG3 ) )
        data.Tangent3 = ReadTangent( reader );
    }

    private Vector4 ReadTangent( BinaryReader reader )
    {
      // TODO: This is a guess. Verify.
      if ( Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TANG ) )
      {
        // 0x2 bytes
        _ = reader.ReadInt16();
        return default;
      }
      else
      {
        var x = reader.ReadSByte().SNormToFloat();
        var y = reader.ReadSByte().SNormToFloat();
        var z = reader.ReadSByte().SNormToFloat();
        var w = reader.ReadSByte().SNormToFloat();

        Assert( x >= -1 && x <= 1, "Tangent X coord out of bounds." );
        Assert( y >= -1 && y <= 1, "Tangent Y coord out of bounds." );
        Assert( z >= -1 && z <= 1, "Tangent Z coord out of bounds." );
        Assert( w >= -1 && w <= 1, "Tangent W coord out of bounds." );

        return new Vector4( x, y, z, w );
      }
    }

    private void ReadColors( BinaryReader reader, S3DInterleavedData data )
    {
      if ( Flags.HasFlag( S3DGeometryBufferFlags._COL0 ) )
        data.Color0 = ReadColor( reader );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._COL1 ) )
        data.Color1 = ReadColor( reader );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._COL2 ) )
        data.Color2 = ReadColor( reader );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._COL3 ) )
        data.Color3 = ReadColor( reader );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._COL4 ) )
        data.Color4 = ReadColor( reader );
    }

    private Vector4 ReadColor( BinaryReader reader )
    {
      // 0x1 bytes
      //var col = reader.ReadByte();
      return default;
    }

    private void ReadUVs( BinaryReader reader, S3DInterleavedData data )
    {
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX0 ) /*|| Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_0 )*/ )
        data.UV0 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_0 ), Flags.HasFlag( S3DGeometryBufferFlags._TEX0_4D ) );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX1 ) /*|| Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_1 )*/ )
        data.UV1 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_1 ), Flags.HasFlag( S3DGeometryBufferFlags._TEX1_4D ) );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX2 ) /*|| Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_2 )*/ )
        data.UV2 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_2 ), Flags.HasFlag( S3DGeometryBufferFlags._TEX2_4D ) );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX3 ) /*|| Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_3 )*/ )
        data.UV3 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_3 ), Flags.HasFlag( S3DGeometryBufferFlags._TEX3_4D ) );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX4 ) /*|| Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_4 )*/ )
        data.UV4 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_4 ), Flags.HasFlag( S3DGeometryBufferFlags._TEX4_4D ) );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX5 ) /*|| Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_5 ) */)
        data.UV5 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_5 ), Flags.HasFlag( S3DGeometryBufferFlags._TEX5_4D ) );

      //if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX0 ) || Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_0 ) )
      //  data.UV0 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_0 ), Flags.HasFlag( S3DGeometryBufferFlags._TEX0_4D ) );
      //if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX1 ) || Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_1 ) )
      //  data.UV1 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_1 ), Flags.HasFlag( S3DGeometryBufferFlags._TEX1_4D ) );
      //if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX2 ) || Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_2 ) )
      //  data.UV2 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_2 ), Flags.HasFlag( S3DGeometryBufferFlags._TEX2_4D ) );
      //if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX3 ) || Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_3 ) )
      //  data.UV3 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_3 ), Flags.HasFlag( S3DGeometryBufferFlags._TEX3_4D ) );
      //if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX4 ) || Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_4 ) )
      //  data.UV4 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_4 ), Flags.HasFlag( S3DGeometryBufferFlags._TEX4_4D ) );
      //if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX5 ) || Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_5 ) )
      //  data.UV5 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_5 ), Flags.HasFlag( S3DGeometryBufferFlags._TEX5_4D ) );
    }

    private Vector4 ReadUV( BinaryReader reader, bool isCompressed, bool is4D )
    {
      if ( is4D )
      {
        // 7 bytes
        //_ = reader.ReadUInt16();
        //_ = reader.ReadUInt16();
        //_ = reader.ReadUInt16();
        //_ = reader.ReadByte();
        //return default;
        return FailReturn<Vector4>( "4D UV Not implemented" );
      }
      else
      {
        // Fairly sure this is correct
        var u = reader.ReadInt16().SNormToFloat();
        var v = 1 - reader.ReadInt16().SNormToFloat();

        Assert( u >= 0 && u <= 1, "UV U coord out of bounds." );
        Assert( v >= 0 && v <= 1, "UV V coord out of bounds." );

        return new Vector4( u, v, 0, 0 );
      }
    }

    #endregion

    private Vector4 DecompressVectorFromInt16( short w )
    {
      /* In common_input.vsh, if the vertex IS compressed, they're unpacking the normal like so:
       *  xz  = (-1.f + 2.f * frac( float2(1.f/181, 1.f/181.0/181.0) * abs(w))) * float2(181.f/179.f, 181.f/180.f);
       *  y   = sign(inInt16Value) * sqrt(saturate(1.f - tmp.x*tmp.x - tmp.z*tmp.z));
       */

      var negativeIdentity = new Vector2( -1.0f );

      var xz = ( negativeIdentity + 2.0f * S3DMath.Frac( new Vector2( 1.0f / 181, 1.0f / 181.0f / 181.0f ) * Math.Abs( w ) ) );
      xz *= new Vector2( 181.0f / 179.0f, 181.0f / 180.0f );

      var yTmp = S3DMath.Sign( w ) * Math.Sqrt( S3DMath.Saturate( 1.0f - xz.X * xz.X - xz.Y * xz.Y ) );

      var x = ( float ) xz.X;
      var y = ( float ) yTmp;
      var z = ( float ) xz.Y;

      return new Vector4( x, y, z, 1 ); // TODO: Should this W be 1?
    }

    private Vector4 DecompressVectorFromFloat( float w )
    {
      /* In common_input.vsh, if the vertex isn't compressed, they're unpacking the normal like so:
       *  norm = -1.0f + 2.f * float3(1/256.0, 1/256.0/256.0, 1/256.0/256.0/256.0) * w);
       */
      var negativeIdentity = new Vector3( -1.0f );
      var divisor = new Vector3( 0.00390625f, 0.0000152587890625f, 0.000000059604644775390625f );
      var result = negativeIdentity + 2.0f * S3DMath.Frac( divisor * w );

      return new Vector4( result, 1 ); // TODO: Verify all this math
    }


  }

}
