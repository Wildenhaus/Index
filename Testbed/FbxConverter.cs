using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.ThreeD;
using Aspose.ThreeD.Entities;
using Aspose.ThreeD.Utilities;
using Saber3D.Data;
using Saber3D.Serializers;

namespace Testbed
{

  public static class FbxConverter
  {
    /* This exports models, but should only serve as a (very sketchy) example.
     * 
     * This is using Aspose3D, a really crappy 3D library that costs a lot of money.
     * It limits you to 50 exports since it's a free trial, and the vector math sucks.
     * I'm only including this file to demonstrate how data should be read.
     */

    public static Stream ConvertTpl( Stream inStream )
    {
      var reader = new BinaryReader( inStream );
      var tpl = new S3DTemplateSerializer().Deserialize( reader );
      var graph = tpl.GeometryGraph;

      var scene = new Scene();

      foreach ( var mesh in graph.Meshes )
        AddMesh( scene, graph, mesh, reader );

      var outStream = new MemoryStream();
      scene.Save( outStream, FileFormat.FBX7700Binary );

      outStream.Position = 0;
      return outStream;
    }

    public static Stream ConvertScn( Stream inStream )
    {
      var reader = new BinaryReader( inStream );
      var scn = new S3DSceneSerializer().Deserialize( reader );
      var graph = scn.GeometryGraph;

      var scene = new Scene();

      //foreach ( var mesh in graph.Meshes )
      //  AddMesh( scene, graph, mesh, reader );

      var n = new Node( scene.Name );
      scene.RootNode.AddChildNode( n );
      foreach ( var submesh in graph.SubMeshes )
      {
        var mesh = graph.Meshes.First( x => x.Id == submesh.MeshId );
        AddSubMesh( n, graph, mesh, submesh, reader );
      }

      var outStream = new MemoryStream();
      scene.Save( outStream, FileFormat.FBX7700Binary );

      outStream.Position = 0;
      return outStream;
    }

    private static void AddMesh( Scene scene, S3DGeometryGraph graph, S3DGeometryMesh mesh, BinaryReader reader )
    {
      var parentObject = graph.Objects[ ( int ) ( mesh.Id + graph.RootNodeIndex ) ];
      var meshNode = new Node( parentObject.Name );
      scene.RootNode.AddChildNode( meshNode );

      var submeshes = graph.SubMeshes.Where( x => x.MeshId == mesh.Id );
      foreach ( var submesh in submeshes )
        AddSubMesh( meshNode, graph, mesh, submesh, reader );
    }

    private static void AddSubMesh( Node parentNode, S3DGeometryGraph graph, S3DGeometryMesh mesh, S3DGeometrySubMesh submesh, BinaryReader reader )
    {
      var buffers = graph.Buffers;
      var parentObject = graph.Objects[ submesh.NodeId ];

      var entity = new Mesh( parentObject.Name );
      var entityNode = new Node( parentObject.Name, entity );
      parentNode.AddChildNode( entityNode );

      var uv1 = entity.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
      var uv2 = entity.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
      var uv3 = entity.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
      var uv4 = entity.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
      var vertexNormals = entity.CreateElement( VertexElementType.Normal, MappingMode.ControlPoint, ReferenceMode.Direct ) as VertexElementNormal;

      var faceMap = new Dictionary<int, int>();

      foreach ( var meshBuffer in mesh.Buffers )
      {
        var buffer = buffers[ ( int ) meshBuffer.BufferId ];
        var startOffset = buffer.StartOffset + meshBuffer.SubBufferOffset;

        var submeshBufferInfo = submesh.BufferInfo;
        switch ( buffer.BufferInfo.BufferType )
        {
          case S3DGeometryBufferType.Face:
          {
            if ( submeshBufferInfo.FaceCount == 0 )
              continue;

            var subBufferOffset = submeshBufferInfo.FaceOffset * buffer.ElementSize;
            reader.BaseStream.Position = startOffset + subBufferOffset;

            if ( buffer.ElementSize == 0xC )
            {
              for ( var i = 0; i < submeshBufferInfo.FaceCount; i++ )
              {
                var faceA = faceMap[ reader.ReadInt32() ];
                var faceB = faceMap[ reader.ReadInt32() ];
                var faceC = faceMap[ reader.ReadInt32() ];
                entity.CreatePolygon( faceA, faceB, faceC );
              }
            }
            else if ( buffer.ElementSize == 0x8 )
            {
              for ( var i = 0; i < submeshBufferInfo.FaceCount; i++ )
              {
                var faceA = faceMap[ reader.ReadUInt16() ];
                var faceB = faceMap[ reader.ReadUInt16() ];
                var faceC = faceMap[ reader.ReadUInt16() ];
                var faceD = faceMap[ reader.ReadUInt16() ];
                entity.CreatePolygon( faceA, faceB, faceC, faceD );
              }
            }
            else if ( buffer.ElementSize == 0x6 )
            {
              for ( var i = 0; i < submeshBufferInfo.FaceCount; i++ )
              {
                var faceA = faceMap[ reader.ReadUInt16() ];
                var faceB = faceMap[ reader.ReadUInt16() ];
                var faceC = faceMap[ reader.ReadUInt16() ];
                entity.CreatePolygon( faceA, faceB, faceC );
              }
            }
            else
              throw new Exception( $"Unknown Face Size: {buffer.ElementSize:X}" );

          }
          break;
          case S3DGeometryBufferType.StaticVert:
          {
            var subBufferOffset = submeshBufferInfo.VertexOffset * buffer.ElementSize;
            reader.BaseStream.Position = startOffset + subBufferOffset;

            var idxStart = submeshBufferInfo.VertexOffset;

            for ( var i = 0; i < submeshBufferInfo.VertexCount; i++ )
            {
              /* Make sure to check buffer.ElementSize before doing this.
               * TPLs typically use Vector3<short>, where each short is compressed via SNorm.
               * LG doesn't seem to use SNorm at all, probably due to the size of the model and
               * the fact that it would lose a lot of precision by doing so.
               */

              if ( buffer.ElementSize == 0x10 )
              {
                var x = reader.ReadSingle();
                var y = reader.ReadSingle();
                var z = reader.ReadSingle();
                var w = reader.ReadSingle(); // Normal?

                entity.ControlPoints.Add( new Vector4( x, y, z, 0 ) );
                faceMap.Add( ( int ) ( idxStart + i ), ( int ) faceMap.Count );
              }
              else if ( buffer.ElementSize == 0x8 )
              {
                var x = SNorm( reader.ReadInt16() );
                var y = SNorm( reader.ReadInt16() );
                var z = SNorm( reader.ReadInt16() );
                var w = reader.ReadInt16(); // Normal packed in W coord

                entity.ControlPoints.Add( new Vector4( x, y, z, 0 ) );
                vertexNormals.Data.Add( UnpackNormFromW( w ) );

                faceMap.Add( ( int ) ( idxStart + i ), ( int ) faceMap.Count );
              }
              else
                throw new Exception( $"Unknown StaticVert Size: {buffer.ElementSize:X}" );
            }
          }
          break;
          case S3DGeometryBufferType.SkinnedVert:
          {
            var subBufferOffset = submeshBufferInfo.VertexOffset * buffer.ElementSize;
            reader.BaseStream.Position = startOffset + subBufferOffset;

            var idxStart = submeshBufferInfo.VertexOffset;

            for ( var i = 0; i < submeshBufferInfo.VertexCount; i++ )
            {
              if ( buffer.ElementSize == 0x10 )
              {
                var x = SNorm( reader.ReadInt16() );
                var y = SNorm( reader.ReadInt16() );
                var z = SNorm( reader.ReadInt16() );
                var w = reader.ReadInt16(); // Normal packed in W coord
                reader.BaseStream.Position -= 8;
                var xyzw = reader.ReadInt64();

                // Weights?
                var unk_01 = reader.ReadByte();
                var unk_02 = reader.ReadByte();
                var unk_03 = reader.ReadByte();
                var unk_04 = reader.ReadByte();

                // Bones?
                var unk_05 = reader.ReadByte();
                var unk_06 = reader.ReadByte();
                var unk_07 = reader.ReadByte();
                var unk_08 = reader.ReadByte();

                entity.ControlPoints.Add( new Vector4( x, y, z, w ) );
                vertexNormals.Data.Add( UnpackNormFromW( w ) );

                faceMap.Add( ( int ) ( idxStart + i ), ( int ) faceMap.Count );
              }
              else
                throw new Exception( $"Unknown SkinnedVert Size: {buffer.ElementSize:X}" );
            }
          }
          break;
          case S3DGeometryBufferType.UV1:
          {
            var subBufferOffset = submeshBufferInfo.VertexOffset * buffer.ElementSize;
            reader.BaseStream.Position = startOffset + subBufferOffset;

            for ( var i = 0; i < submeshBufferInfo.VertexCount; i++ )
            {
              if ( buffer.ElementSize == 0x10 )
              {
                // TODO: THIS IS A GUESS
                var unk_01 = reader.ReadInt32();
                var unk_02 = reader.ReadInt32();
                var unk_03 = reader.ReadInt32();

                var u = SNorm( reader.ReadInt16() );
                var v = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped

                uv1.Data.Add( new Vector4( u, v, 0, 0 ) );
              }
              else if ( buffer.ElementSize == 0xC )
              {
                // TODO: THIS IS A GUESS
                var unk_01 = reader.ReadInt32();
                var unk_02 = reader.ReadInt32();

                var u = SNorm( reader.ReadInt16() );
                var v = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped

                uv1.Data.Add( new Vector4( u, v, 0, 0 ) );
              }
              else if ( buffer.ElementSize == 0x8 )
              {
                var unk_01 = reader.ReadInt32();

                var u = SNorm( reader.ReadInt16() );
                var v = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped

                uv1.Data.Add( new Vector4( u, v, 0, 0 ) );
              }
              else
                throw new Exception( $"Unknown UV1 Size: {buffer.ElementSize:X}" );
            }
          }
          break;
          case S3DGeometryBufferType.UV2:
          {
            var subBufferOffset = submeshBufferInfo.VertexOffset * buffer.ElementSize;
            reader.BaseStream.Position = startOffset + subBufferOffset;

            for ( var i = 0; i < submeshBufferInfo.VertexCount; i++ )
            {
              if ( buffer.ElementSize == 0x18 )
              {
                // TODO: THIS IS A GUESS
                var unk_01 = reader.ReadInt32();
                var unk_02 = reader.ReadInt32();
                var unk_03 = reader.ReadInt32();
                var unk_04 = reader.ReadInt32();

                var u1 = SNorm( reader.ReadInt16() );
                var v1 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u2 = SNorm( reader.ReadInt16() );
                var v2 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped

                uv1.Data.Add( new Vector4( u1, v1, 0, 0 ) );
                uv2.Data.Add( new Vector4( u2, v2, 0, 0 ) );
              }
              else if ( buffer.ElementSize == 0x14 )
              {
                // TODO: THIS IS A GUESS
                var unk_01 = reader.ReadInt32();
                var unk_02 = reader.ReadInt32();
                var unk_03 = reader.ReadInt32();

                var u1 = SNorm( reader.ReadInt16() );
                var v1 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u2 = SNorm( reader.ReadInt16() );
                var v2 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped

                uv1.Data.Add( new Vector4( u1, v1, 0, 0 ) );
                uv2.Data.Add( new Vector4( u2, v2, 0, 0 ) );
              }
              else if ( buffer.ElementSize == 0x10 )
              {
                var unk_01 = reader.ReadInt32();
                var unk_02 = reader.ReadInt32();

                var u1 = SNorm( reader.ReadInt16() );
                var v1 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u2 = SNorm( reader.ReadInt16() );
                var v2 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped

                uv1.Data.Add( new Vector4( u1, v1, 0, 0 ) );
                uv2.Data.Add( new Vector4( u2, v2, 0, 0 ) );
              }
              else
                throw new Exception( $"Unknown UV2 Size: {buffer.ElementSize:X}" );
            }
          }
          break;
          case S3DGeometryBufferType.UV3:
          {
            var subBufferOffset = submeshBufferInfo.VertexOffset * buffer.ElementSize;
            reader.BaseStream.Position = startOffset + subBufferOffset;

            for ( var i = 0; i < submeshBufferInfo.VertexCount; i++ )
            {
              if ( buffer.ElementSize == 0x28 )
              {
                // TODO: THIS IS A GUESS
                var unk_01 = reader.ReadInt32();
                var unk_02 = reader.ReadInt32();
                var unk_03 = reader.ReadInt32();
                var unk_04 = reader.ReadInt32();
                var unk_05 = reader.ReadInt32();
                var unk_06 = reader.ReadInt32();

                var u1 = SNorm( reader.ReadInt16() );
                var v1 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u2 = SNorm( reader.ReadInt16() );
                var v2 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u3 = SNorm( reader.ReadInt16() );
                var v3 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped

                uv1.Data.Add( new Vector4( u1, v1, 0, 0 ) );
                uv2.Data.Add( new Vector4( u2, v2, 0, 0 ) );
                uv3.Data.Add( new Vector4( u3, v3, 0, 0 ) );
              }
              else if ( buffer.ElementSize == 0x20 )
              {
                // TODO: THIS IS A GUESS
                var unk_01 = reader.ReadInt32();
                var unk_02 = reader.ReadInt32();
                var unk_03 = reader.ReadInt32();
                var unk_04 = reader.ReadInt32();
                var unk_05 = reader.ReadInt32();

                var u1 = SNorm( reader.ReadInt16() );
                var v1 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u2 = SNorm( reader.ReadInt16() );
                var v2 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u3 = SNorm( reader.ReadInt16() );
                var v3 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped

                uv1.Data.Add( new Vector4( u1, v1, 0, 0 ) );
                uv2.Data.Add( new Vector4( u2, v2, 0, 0 ) );
                uv3.Data.Add( new Vector4( u3, v3, 0, 0 ) );
              }
              else if ( buffer.ElementSize == 0x1C )
              {
                // TODO: THIS IS A GUESS
                var unk_01 = reader.ReadInt32();
                var unk_02 = reader.ReadInt32();
                var unk_03 = reader.ReadInt32();
                var unk_04 = reader.ReadInt32();

                var u1 = SNorm( reader.ReadInt16() );
                var v1 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u2 = SNorm( reader.ReadInt16() );
                var v2 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u3 = SNorm( reader.ReadInt16() );
                var v3 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped

                uv1.Data.Add( new Vector4( u1, v1, 0, 0 ) );
                uv2.Data.Add( new Vector4( u2, v2, 0, 0 ) );
                uv3.Data.Add( new Vector4( u3, v3, 0, 0 ) );
              }
              else if ( buffer.ElementSize == 0x18 )
              {
                var unk_01 = reader.ReadInt32();
                var unk_02 = reader.ReadInt32();
                var unk_03 = reader.ReadInt32();

                var u1 = SNorm( reader.ReadInt16() );
                var v1 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u2 = SNorm( reader.ReadInt16() );
                var v2 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u3 = SNorm( reader.ReadInt16() );
                var v3 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped

                uv1.Data.Add( new Vector4( u1, v1, 0, 0 ) );
                uv2.Data.Add( new Vector4( u2, v2, 0, 0 ) );
                uv3.Data.Add( new Vector4( u3, v3, 0, 0 ) );
              }
              else
                throw new Exception( $"Unknown UV3 Size: {buffer.ElementSize:X}" );
            }
          }
          break;
          case S3DGeometryBufferType.UV4:
          {
            var subBufferOffset = submeshBufferInfo.VertexOffset * buffer.ElementSize;
            reader.BaseStream.Position = startOffset + subBufferOffset;

            for ( var i = 0; i < submeshBufferInfo.VertexCount; i++ )
            {
              if ( buffer.ElementSize == 0x2C )
              {
                // TODO: THIS IS A GUESS
                var unk_01 = reader.ReadInt32();
                var unk_02 = reader.ReadInt32();
                var unk_03 = reader.ReadInt32();
                var unk_04 = reader.ReadInt32();
                var unk_05 = reader.ReadInt32();
                var unk_06 = reader.ReadInt32();
                var unk_07 = reader.ReadInt32();

                var u1 = SNorm( reader.ReadInt16() );
                var v1 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u2 = SNorm( reader.ReadInt16() );
                var v2 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u3 = SNorm( reader.ReadInt16() );
                var v3 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u4 = SNorm( reader.ReadInt16() );
                var v4 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped

                uv1.Data.Add( new Vector4( u1, v1, 0, 0 ) );
                uv2.Data.Add( new Vector4( u2, v2, 0, 0 ) );
                uv3.Data.Add( new Vector4( u3, v3, 0, 0 ) );
                uv4.Data.Add( new Vector4( u4, v4, 0, 0 ) );
              }
              else if ( buffer.ElementSize == 0x28 )
              {
                // TODO: THIS IS A GUESS
                var unk_01 = reader.ReadInt32();
                var unk_02 = reader.ReadInt32();
                var unk_03 = reader.ReadInt32();
                var unk_04 = reader.ReadInt32();
                var unk_05 = reader.ReadInt32();
                var unk_06 = reader.ReadInt32();

                var u1 = SNorm( reader.ReadInt16() );
                var v1 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u2 = SNorm( reader.ReadInt16() );
                var v2 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u3 = SNorm( reader.ReadInt16() );
                var v3 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u4 = SNorm( reader.ReadInt16() );
                var v4 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped

                uv1.Data.Add( new Vector4( u1, v1, 0, 0 ) );
                uv2.Data.Add( new Vector4( u2, v2, 0, 0 ) );
                uv3.Data.Add( new Vector4( u3, v3, 0, 0 ) );
                uv4.Data.Add( new Vector4( u4, v4, 0, 0 ) );
              }
              else if ( buffer.ElementSize == 0x24 )
              {
                // TODO: THIS IS A GUESS
                var unk_01 = reader.ReadInt32();
                var unk_02 = reader.ReadInt32();
                var unk_03 = reader.ReadInt32();
                var unk_04 = reader.ReadInt32();
                var unk_05 = reader.ReadInt32();

                var u1 = SNorm( reader.ReadInt16() );
                var v1 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u2 = SNorm( reader.ReadInt16() );
                var v2 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u3 = SNorm( reader.ReadInt16() );
                var v3 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u4 = SNorm( reader.ReadInt16() );
                var v4 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped

                uv1.Data.Add( new Vector4( u1, v1, 0, 0 ) );
                uv2.Data.Add( new Vector4( u2, v2, 0, 0 ) );
                uv3.Data.Add( new Vector4( u3, v3, 0, 0 ) );
                uv4.Data.Add( new Vector4( u4, v4, 0, 0 ) );
              }
              else if ( buffer.ElementSize == 0x20 )
              {
                var unk_01 = reader.ReadInt32();
                var unk_02 = reader.ReadInt32();
                var unk_03 = reader.ReadInt32();
                var unk_04 = reader.ReadInt32();

                var u1 = SNorm( reader.ReadInt16() );
                var v1 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u2 = SNorm( reader.ReadInt16() );
                var v2 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u3 = SNorm( reader.ReadInt16() );
                var v3 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped
                var u4 = SNorm( reader.ReadInt16() );
                var v4 = 1 - SNorm( reader.ReadInt16() ); // V coord is flipped

                uv1.Data.Add( new Vector4( u1, v1, 0, 0 ) );
                uv2.Data.Add( new Vector4( u2, v2, 0, 0 ) );
                uv3.Data.Add( new Vector4( u3, v3, 0, 0 ) );
                uv4.Data.Add( new Vector4( u4, v4, 0, 0 ) );
              }
              else
                throw new Exception( $"Unknown UV4 Size: {buffer.ElementSize:X}" );
            }
          }
          break;
        }
      }

      /* TODO:
       * I have these commented out because they can sometimes result in a scale of 0.
       * This can result in the meshes not appearing after import.
       * This seems to be an issue primarily with level geometry.
       * 
       * In contrast, TPL models are often skewed/scaled incorrectly without this being applied.
       */

      // Transforms
      //var pos = submesh.Position;
      //entityNode.Transform.Translation = new Vector3( pos.X, pos.Y, pos.Z );

      //var scale = submesh.Scale;
      //entityNode.Transform.Scale = new Vector3( scale.X, scale.Y, scale.Z );
    }

    private static float SNorm( this short value )
      => value / ( float ) short.MaxValue;

    private static Vector4 UnpackNormFromW( in short w )
    {
      // This seems close, but there are a lot of issues with duplicate vertices
      // pointing in opposite directions, causing the normals to be screwed up.
      // It may have to do with backfacing.

      float sign( short value )
        => value > 0 ? 1 : value < 0 ? -1 : 0;

      float frac( float value )
        => value % 1;

      var x = ( -1f + 2f * frac( ( 1.0f / 181 ) * Math.Abs( w ) ) ) * ( 181.0f / 179f );
      var z = ( -1f + 2f * frac( ( 1.0f / 181.0f / 181.0f ) * Math.Abs( w ) ) ) * ( 181.0f / 180.0f );
      var y = sign( w ) * MathF.Sqrt( 1.0f - ( x * x ) - ( z * z ) );

      return new Vector4( x, y, z, sign( w ) );
    }

  }

}
