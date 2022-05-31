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
    /* This is using Aspose3D, a really crappy 3D library that costs a lot of money.
     * It limits you to 50 exports per application run, and the vector math sucks.
     * I'm only including this file to demonstrate how data should be read.
     */

    public static Stream Convert( Stream inStream )
    {
      var reader = new BinaryReader( inStream );
      var scn = new S3DSceneSerializer().Deserialize( reader );

      var scene = new Scene();

      foreach ( var mesh in scn.GeometryGraph.Meshes )
        AddMesh( scene, scn, mesh, reader );

      var outStream = new MemoryStream();
      scene.Save( outStream, FileFormat.FBX7700Binary );

      outStream.Position = 0;
      return outStream;
    }

    private static void AddMesh( Scene scene, S3DScene scn, S3DGeometryMesh mesh, BinaryReader reader )
    {
      var meshNode = new Node();
      scene.RootNode.AddChildNode( meshNode );

      var submeshes = scn.GeometryGraph.SubMeshes.Where( x => x.MeshId == mesh.Id );
      foreach ( var submesh in submeshes )
      {
        AddSubMesh( meshNode, scn, mesh, submesh, reader );
      }
    }

    private static void AddSubMesh( Node meshNode, S3DScene scn, S3DGeometryMesh mesh, S3DGeometrySubMesh submesh, BinaryReader reader )
    {
      var buffers = scn.GeometryGraph.Buffers;

      var entity = new Mesh();
      var entityNode = new Node( "", entity );
      meshNode.AddChildNode( entityNode );

      foreach ( var meshBuffer in mesh.Buffers )
      {
        var buffer = buffers[ ( int ) meshBuffer.BufferId ];
        var startOffset = buffer.StartOffset + meshBuffer.SubBufferOffset;

        var submeshBufferInfo = submesh.BufferInfo;
        switch ( buffer.BufferInfo.BufferType )
        {
          case S3DGeometryBufferType.Face:
          {
            // TODO
            /* THIS IS REALLY BAD
             * I'm just attempting to adjust the indices of each face's vertices
             * based on a min. This is probably why there are random broken meshes.
             * Not an implementation for release.
             */
            var subBufferOffset = submeshBufferInfo.FaceOffset * buffer.ElementSize;
            reader.BaseStream.Position = startOffset + subBufferOffset;

            if ( submeshBufferInfo.FaceCount == 0 )
              continue;

            // Load Faces
            var faces = new List<short[]>();
            for ( var i = 0; i < submeshBufferInfo.FaceCount; i++ )
              faces.Add( new[] { reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16() } );

            // Normalize the indices
            var min = faces.Min( x => x.Min() );
            foreach ( var f in faces )
              for ( var i = 0; i < f.Length; i++ )
                f[ i ] -= min;

            foreach ( var f in faces )
              entity.CreatePolygon( f[ 0 ], f[ 1 ], f[ 2 ] );
          }
          break;
          case S3DGeometryBufferType.StaticVert:
          {
            var subBufferOffset = submeshBufferInfo.VertexOffset * buffer.ElementSize;
            reader.BaseStream.Position = startOffset + subBufferOffset;

            for ( var i = 0; i < submeshBufferInfo.VertexCount; i++ )
            {
              //TODO
              /* Make sure to check buffer.ElementSize before doing this.
               * TPLs typically use Vector3<short>, where each short is compressed via SNorm.
               * LG doesn't seem to use SNorm at all, probably due to the size of the model and
               * the fact that it would lose a lot of precision by doing so.
               */

              var x = reader.ReadSingle();
              var y = reader.ReadSingle();
              var z = reader.ReadSingle();
              var w = reader.ReadSingle();

              entity.ControlPoints.Add( new Vector4( x, y, z, w ) );
            }
          }
          break;
        }
      }

      // Transforms
      //var pos = submesh.Position;
      //entityNode.Transform.Translation = new Vector3( pos.X, pos.Y, pos.Z );

      //var scale = submesh.Scale;
      //entityNode.Transform.Scale = new Vector3( scale.X, scale.Y, scale.Z );
    }

    private static float SNorm( this short value )
      => value / ( float ) short.MaxValue;

  }

}
