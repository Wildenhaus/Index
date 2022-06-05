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

      // TODO: Not sure what these are. Color?
      if ( Flags.HasFlag( S3DGeometryBufferFlags.Unk_17 ) )
        _ = reader.ReadInt32();
      if ( Flags.HasFlag( S3DGeometryBufferFlags.Unk_18 ) )
        _ = reader.ReadInt32();
      if ( Flags.HasFlag( S3DGeometryBufferFlags.Unk_19 ) )
        _ = reader.ReadInt32();

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
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TANG4 ) )
        data.Tangent4 = ReadTangent( reader );
    }

    private Vector4 ReadTangent( BinaryReader reader )
    {
      var x = reader.ReadSByte().SNormToFloat();
      var y = reader.ReadSByte().SNormToFloat();
      var z = reader.ReadSByte().SNormToFloat();
      var w = reader.ReadSByte().SNormToFloat();

      Assert( x >= -1.01 && x <= 1.01, "Tangent X coord out of bounds." );
      Assert( y >= -1.01 && y <= 1.01, "Tangent Y coord out of bounds." );
      Assert( z >= -1.01 && z <= 1.01, "Tangent Z coord out of bounds." );
      Assert( w >= -1.01 && w <= 1.01, "Tangent W coord out of bounds." );

      return new Vector4( x, y, z, w );
    }

    private void ReadUVs( BinaryReader reader, S3DInterleavedData data )
    {
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX0 ) /*|| Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_0 )*/ )
        data.UV0 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_0 ) );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX1 ) /*|| Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_1 )*/ )
        data.UV1 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_1 ) );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX2 ) /*|| Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_2 )*/ )
        data.UV2 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_2 ) );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX3 ) /*|| Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_3 )*/ )
        data.UV3 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_3 ) );
      if ( Flags.HasFlag( S3DGeometryBufferFlags._TEX4 ) /*|| Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_4 )*/ )
        data.UV4 = ReadUV( reader, Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_TEX_4 ) );

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

    private Vector4 ReadUV( BinaryReader reader, bool isCompressed )
    {
      if ( isCompressed )
      {
        // Fairly sure this is correct
        var u = reader.ReadInt16().SNormToFloat();
        var v = 1 - reader.ReadInt16().SNormToFloat();

        return new Vector4( u, v, 0, 0 );
      }
      else
      {
        // Fairly sure this is correct
        var u = reader.ReadSingle();
        var v = 1 - reader.ReadSingle();

        return new Vector4( u, v, 0, 0 );
      }
    }

    #endregion

  }

}
