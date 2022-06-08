using System;
using System.IO;
using System.Numerics;
using Saber3D.Common;
using Saber3D.Data;
using Saber3D.Data.Geometry;
using static Saber3D.Assertions;

namespace Saber3D.Serializers.Geometry
{

  public class S3DVertexSerializer
  {

    #region Properties

    private S3DGeometryBuffer Buffer { get; }

    private S3DGeometryBufferFlags Flags
    {
      get => Buffer.Flags;
    }

    #endregion

    #region Constructor

    public S3DVertexSerializer( S3DGeometryBuffer buffer )
    {
      Buffer = buffer;
    }

    #endregion

    #region Public Methods

    public S3DVertex Deserialize( BinaryReader reader )
    {
      Assert( Flags.HasFlag( S3DGeometryBufferFlags._VERT ),
        "Buffer does not specify _VERT in its flags." );

      if ( HasSkinningData() )
        return DeserializeSkinnedVertex( reader );
      else
        return DeserializeStaticVertex( reader );
    }

    #endregion

    #region Private Methods

    private bool HasSkinningData()
    {
      return Flags.HasFlag( S3DGeometryBufferFlags._WEIGHT1 )
          || Flags.HasFlag( S3DGeometryBufferFlags._WEIGHT2 )
          || Flags.HasFlag( S3DGeometryBufferFlags._WEIGHT3 )
          || Flags.HasFlag( S3DGeometryBufferFlags._WEIGHT4 )
          || Flags.HasFlag( S3DGeometryBufferFlags._INDEX );
    }

    private bool HasNormal()
    {
      return Flags.HasFlag( S3DGeometryBufferFlags._NORM )
          || Flags.HasFlag( S3DGeometryBufferFlags._NORM_IN_VERT4 )
          || Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_NORM );
    }

    private S3DVertexSkinned DeserializeSkinnedVertex( BinaryReader reader )
    {
      var vertex = new S3DVertexSkinned();

      ReadVertexPosition( reader, vertex );
      ReadVertexNormal( reader, vertex );
      ReadVertexSkinningData( reader, vertex );
      ApplyVertexTransforms( reader, vertex );

      return vertex;
    }

    private S3DVertexStatic DeserializeStaticVertex( BinaryReader reader )
    {
      var vertex = new S3DVertexStatic();

      ReadVertexPosition( reader, vertex );
      ReadVertexNormal( reader, vertex );
      ApplyVertexTransforms( reader, vertex );

      return vertex;
    }

    private void ReadVertexPosition( BinaryReader reader, S3DVertex vertex )
    {
      if ( Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_VERT ) )
      {
        /* I'm trying to follow the code in the vertex shader as close as possible.
         * If the vertex is compressed to an int16, they leave it be for now.
         */
        vertex.Position = new Vector4(
          x: reader.ReadInt16().SNormToFloat(),
          y: reader.ReadInt16().SNormToFloat(),
          z: reader.ReadInt16().SNormToFloat(),
          w: 1
          );
      }
      else
      {
        vertex.Position = new Vector4(
          x: reader.ReadSingle(),
          y: reader.ReadSingle(),
          z: reader.ReadSingle(),
          w: 1
          );
      }
    }

    private void ReadVertexNormal( BinaryReader reader, S3DVertex vertex )
    {
      if ( !HasNormal() )
        return;

      if ( Flags.HasFlag( S3DGeometryBufferFlags._NORM_IN_VERT4 ) )
      {
        if ( Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_VERT ) )
          vertex.Normal = DecompressNormalFromInt16( reader.ReadInt16() );
        else
          vertex.Normal = DecompressNormalFromFloat( reader.ReadSingle() );
      }
      else if ( Flags.HasFlag( S3DGeometryBufferFlags._NORM ) )
      {
        // TODO: If not _NORM_IN_VERT4, we should be reading the normal from somewhere.
        if ( Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_NORM ) )
          vertex.Normal = new Vector4( -1.0f ) + 2.0f * vertex.Normal;
      }
      else if ( Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_NORM ) )
      {
        // TODO: This is a guess
        // This is to handle cases where _NORM isn't defined but _COMPRESSED_NORM is.
        // So far this seems correct...
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        var z = reader.ReadSingle();
        vertex.Normal = new Vector4( x, y, z, 1 );
      }
    }

    private void ReadVertexSkinningData( BinaryReader reader, S3DVertexSkinned vertex )
    {
      // TODO: Idk if these data types are right, and/or what to do with them.
      if ( Flags.HasFlag( S3DGeometryBufferFlags._WEIGHT1 ) )
        vertex.Weight1 = reader.ReadByte();
      if ( Flags.HasFlag( S3DGeometryBufferFlags._WEIGHT2 ) )
        vertex.Weight2 = reader.ReadByte();
      if ( Flags.HasFlag( S3DGeometryBufferFlags._WEIGHT3 ) )
        vertex.Weight3 = reader.ReadByte();
      if ( Flags.HasFlag( S3DGeometryBufferFlags._WEIGHT4 ) )
        vertex.Weight4 = reader.ReadByte();
      if ( Flags.HasFlag( S3DGeometryBufferFlags._INDEX ) )
        vertex.Index = reader.ReadUInt32();
    }

    private void ApplyVertexTransforms( BinaryReader reader, S3DVertex vertex )
    {
      if ( Flags.HasFlag( S3DGeometryBufferFlags._VERT_2D ) )
      {
        /* This appears to be used for UI elements only.
         * Commented out because it's making a lot of models flat.
         */

        //var pos = vertex.Position;
        //vertex.Position = new Vector4( pos.X, pos.Y, 0, 1 );
      }
      else
      {
        if ( Flags.HasFlag( S3DGeometryBufferFlags._COMPRESSED_VERT ) )
        {
          // TODO
          /* common_input.vsh uses two variables to offset and scale the vertex:
           *   Out.Pos.xyz = comprOffset + (In.Pos.xyz * (comprScale / 32500.f));
           *   Out.Pos.w = 1;
           */
        }
      }
    }

    private Vector4 DecompressNormalFromInt16( short w )
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

      Assert( x < 1.1f && x > -1.1f );
      Assert( y < 1.1f && y > -1.1f );
      Assert( z < 1.1f && z > -1.1f );
      Assert( !float.IsNaN( x ) );
      Assert( !float.IsNaN( y ) );
      Assert( !float.IsNaN( z ) );

      return new Vector4( x, y, z, S3DMath.Sign( w ) ); // TODO: Should this W be 1?
    }

    private Vector4 DecompressNormalFromFloat( float w )
    {
      /* In common_input.vsh, if the vertex isn't compressed, they're unpacking the normal like so:
       *  norm = -1.0f + 2.f * float3(1/256.0, 1/256.0/256.0, 1/256.0/256.0/256.0) * w);
       */
      var negativeIdentity = new Vector3( -1.0f );
      var divisor = new Vector3( 0.00390625f, 0.0000152587890625f, 0.000000059604644775390625f );
      var result = negativeIdentity + 2.0f * S3DMath.Frac( divisor * w );

      return new Vector4( result, 1 ); // TODO: Verify all this math
    }

    #endregion

  }

}
