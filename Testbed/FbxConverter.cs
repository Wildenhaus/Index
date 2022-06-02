using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.ThreeD;
using Aspose.ThreeD.Entities;
using Aspose.ThreeD.Utilities;
using Saber3D.Data;
using Saber3D.Data.Geometry;
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
      var vertCount = 0;

      foreach ( var meshBuffer in mesh.Buffers )
      {
        var buffer = buffers[ ( int ) meshBuffer.BufferId ];
        var startOffset = buffer.StartOffset + meshBuffer.SubBufferOffset;

        var submeshBufferInfo = submesh.BufferInfo;

        switch ( buffer.ElementType )
        {
          case S3DGeometryElementType.Vertex:
          {
            var startIndex = submeshBufferInfo.VertexOffset + meshBuffer.SubBufferOffset / buffer.ElementSize;
            var endIndex = startIndex + submeshBufferInfo.VertexCount;

            for ( var i = startIndex; i < endIndex; i++ )
            {
              var element = buffer.Elements[ i ] as S3DVertex;
              var vert = new Vector4( element.X, element.Y, element.Z );
              var norm = new Vector4( element.Normal.X, element.Normal.Y, element.Normal.Z );

              entity.ControlPoints.Add( vert );
              vertexNormals.Data.Add( norm );
              faceMap.Add( ( int ) i, faceMap.Count );
            }

            vertCount += submeshBufferInfo.VertexCount;
          }
          break;
          case S3DGeometryElementType.Face:
          {
            var startIndex = submeshBufferInfo.FaceOffset + meshBuffer.SubBufferOffset / buffer.ElementSize;
            var endIndex = startIndex + submeshBufferInfo.FaceCount;

            for ( var i = startIndex; i < endIndex; i++ )
            {
              var element = buffer.Elements[ i ] as S3DFace;


              entity.CreatePolygon( element.Select( x => faceMap[ x ] ).ToArray() );
              //switch ( element )
              //{
              //  case S3DEdge edge:
              //    break;
              //  case S3DFaceTri tri:
              //    entity.CreatePolygon( faceMap[ tri.VertexA ], faceMap[ tri.VertexB ], faceMap[ tri.VertexC ] );
              //    break;
              //  case S3DFaceQuad quad:
              //    entity.CreatePolygon( faceMap[ quad.VertexA ], faceMap[ quad.VertexB ], faceMap[ quad.VertexC ], faceMap[ quad.VertexD ] );
              //    break;
              //  case S3DFaceNgon ngon:
              //    entity.CreatePolygon( ngon.Select( x => faceMap[ x ] ).Cast<int>().ToArray() );
              //    break;
              //}
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

  }

}
