using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.ThreeD;
using Aspose.ThreeD.Entities;
using Aspose.ThreeD.Shading;
using Aspose.ThreeD.Utilities;
using Saber3D.Data;
using Saber3D.Data.Geometry;
using Saber3D.Data.Materials;
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
      MaterialCache.Clear();

      var reader = new BinaryReader( inStream );
      var tpl = new S3DTemplateSerializer().Deserialize( reader );
      var graph = tpl.GeometryGraph;

      var scene = new Scene();

      foreach ( var mesh in graph.Meshes )
        AddMesh( scene, graph, mesh, reader );

      var outStream = new MemoryStream();
      scene.Save( outStream, FileFormat.Collada );

      outStream.Position = 0;
      return outStream;
    }

    public static Stream ConvertScn( Stream inStream )
    {
      MaterialCache.Clear();

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
      scene.Save( outStream, FileFormat.Collada );

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

      VertexElementTangent tan0 = null;
      VertexElementTangent tan1 = null;
      VertexElementTangent tan2 = null;
      VertexElementTangent tan3 = null;
      VertexElementTangent tan4 = null;
      VertexElementUV uv0 = null;
      VertexElementUV uv1 = null;
      VertexElementUV uv2 = null;
      VertexElementUV uv3 = null;
      VertexElementUV uv4 = null;
      VertexElementNormal vertexNormals = null;

      var faceMap = new Dictionary<int, int>();

      var vertBuffers = mesh.Buffers.Select( x => buffers[ ( int ) x.BufferId ] ).Where( x => x.ElementType == S3DGeometryElementType.Vertex ).ToArray();
      if ( vertBuffers.Length > 1 )
        throw new System.Exception();

      foreach ( var meshBuffer in mesh.Buffers )
      {
        var buffer = buffers[ ( int ) meshBuffer.BufferId ];
        var startOffset = buffer.StartOffset + meshBuffer.SubBufferOffset;

        var submeshBufferInfo = submesh.BufferInfo;

        switch ( buffer.ElementType )
        {
          case S3DGeometryElementType.Vertex:
          {
            var offset = submeshBufferInfo.VertexOffset;
            var startIndex = offset + meshBuffer.SubBufferOffset / buffer.ElementSize;
            var endIndex = startIndex + submeshBufferInfo.VertexCount;

            for ( var i = startIndex; i < endIndex; i++ )
            {
              var element = buffer.Elements[ i ] as S3DVertex;
              var vert = new Vector4( element.X, element.Y, element.Z );
              entity.ControlPoints.Add( vert );

              if ( element.Normal.HasValue )
              {
                if ( vertexNormals is null )
                  vertexNormals = entity.CreateElement( VertexElementType.Normal, MappingMode.ControlPoint, ReferenceMode.Direct ) as VertexElementNormal;

                var norm = new Vector4( element.Normal.Value.X, element.Normal.Value.Y, element.Normal.Value.Z );
                vertexNormals.Data.Add( norm );
              }
              faceMap.Add( ( int ) offset++, faceMap.Count );
            }

          }
          break;
          case S3DGeometryElementType.Face:
          {
            var startIndex = submeshBufferInfo.FaceOffset + meshBuffer.SubBufferOffset / buffer.ElementSize;
            var endIndex = startIndex + submeshBufferInfo.FaceCount;

            var faces = buffer.Elements.Skip( ( int ) startIndex ).Take( submeshBufferInfo.FaceCount ).Cast<S3DFace>();
            var faceIds = faces.SelectMany( x => x );
            var min = faceIds.Min( x => x );
            var max = faceIds.Max( x => x );
            if ( min != submeshBufferInfo.VertexOffset )
              throw new System.Exception();
            if ( buffer.ElementSize != 6 )
              throw new System.Exception();

            for ( var i = startIndex; i < endIndex; i++ )
            {
              var element = buffer.Elements[ i ] as S3DFace;

              entity.CreatePolygon( faceMap[ element[ 0 ] ], faceMap[ element[ 1 ] ], faceMap[ element[ 2 ] ] );
            }
          }
          break;
          case S3DGeometryElementType.Interleaved:
          {
            var startIndex = submeshBufferInfo.VertexOffset + meshBuffer.SubBufferOffset / buffer.ElementSize;
            var endIndex = startIndex + submeshBufferInfo.VertexCount;

            for ( var i = startIndex; i < endIndex; i++ )
            {
              var element = buffer.Elements[ i ] as S3DInterleavedData;

              if ( element.Tangent0.HasValue )
              {
                if ( tan0 is null )
                  tan0 = entity.CreateElement( VertexElementType.Tangent, MappingMode.ControlPoint, ReferenceMode.Direct ) as VertexElementTangent;
                var t = element.Tangent0.Value;
                tan0.Data.Add( new Vector4( t.X, t.Y, t.Z, t.W ) );
              }
              if ( element.Tangent1.HasValue )
              {
                if ( tan1 is null )
                  tan1 = entity.CreateElement( VertexElementType.Tangent, MappingMode.ControlPoint, ReferenceMode.Direct ) as VertexElementTangent;
                var t = element.Tangent1.Value;
                tan1.Data.Add( new Vector4( t.X, t.Y, t.Z, t.W ) );
              }
              if ( element.Tangent2.HasValue )
              {
                if ( tan2 is null )
                  tan2 = entity.CreateElement( VertexElementType.Tangent, MappingMode.ControlPoint, ReferenceMode.Direct ) as VertexElementTangent;
                var t = element.Tangent2.Value;
                tan2.Data.Add( new Vector4( t.X, t.Y, t.Z, t.W ) );
              }
              if ( element.Tangent3.HasValue )
              {
                if ( tan3 is null )
                  tan3 = entity.CreateElement( VertexElementType.Tangent, MappingMode.ControlPoint, ReferenceMode.Direct ) as VertexElementTangent;
                var t = element.Tangent3.Value;
                tan3.Data.Add( new Vector4( t.X, t.Y, t.Z, t.W ) );
              }
              if ( element.Tangent4.HasValue )
              {
                if ( tan4 is null )
                  tan4 = entity.CreateElement( VertexElementType.Tangent, MappingMode.ControlPoint, ReferenceMode.Direct ) as VertexElementTangent;
                var t = element.Tangent4.Value;
                tan4.Data.Add( new Vector4( t.X, t.Y, t.Z, t.W ) );
              }

              if ( element.UV0.HasValue )
              {
                if ( uv0 is null )
                  uv0 = entity.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
                var uv = element.UV0.Value;
                uv0.Data.Add( new Vector4( uv.X, uv.Y, uv.Z, uv.W ) );
              }
              if ( element.UV1.HasValue )
              {
                if ( uv1 is null )
                  uv1 = entity.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
                var uv = element.UV1.Value;
                uv1.Data.Add( new Vector4( uv.X, uv.Y, uv.Z, uv.W ) );
              }
              if ( element.UV2.HasValue )
              {
                if ( uv2 is null )
                  uv2 = entity.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
                var uv = element.UV2.Value;
                uv2.Data.Add( new Vector4( uv.X, uv.Y, uv.Z, uv.W ) );
              }
              if ( element.UV3.HasValue )
              {
                if ( uv3 is null )
                  uv3 = entity.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
                var uv = element.UV3.Value;
                uv3.Data.Add( new Vector4( uv.X, uv.Y, uv.Z, uv.W ) );
              }
              if ( element.UV4.HasValue )
              {
                if ( uv4 is null )
                  uv4 = entity.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
                var uv = element.UV4.Value;
                uv4.Data.Add( new Vector4( uv.X, uv.Y, uv.Z, uv.W ) );
              }
            }

          }
          break;
          case S3DGeometryElementType.Unknown:
            break;
        }

      }

      ApplySubMeshMaterials( entityNode, submesh );

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

    // TODO: Make this non-static
    private static Dictionary<string, PbrSpecularMaterial> MaterialCache = new Dictionary<string, PbrSpecularMaterial>();

    private static void ApplySubMeshMaterials( Node entityNode, S3DGeometrySubMesh submesh )
    {
      if ( submesh.Material is null )
        return;

      if ( submesh.Material.Layer0 != null )
        ApplySubMeshMaterial( entityNode, submesh.Material.Layer0 );
      if ( submesh.Material.Layer1 != null )
        ApplySubMeshMaterial( entityNode, submesh.Material.Layer1 );
      if ( submesh.Material.Layer2 != null )
        ApplySubMeshMaterial( entityNode, submesh.Material.Layer2 );
      if ( submesh.Material.Layer3 != null )
        ApplySubMeshMaterial( entityNode, submesh.Material.Layer3 );
    }

    private static void ApplySubMeshMaterial( Node entityNode, S3DMaterialLayer layer )
    {
      if ( MaterialCache.TryGetValue( layer.TextureName, out var mat ) )
      {
        entityNode.Materials.Add( mat );
        return;
      }

      mat = new PbrSpecularMaterial();
      mat.Name = layer.TextureName;

      mat.DiffuseTexture = new Texture( layer.TextureName + ".png" );
      mat.NormalTexture = new TextureBase( layer.TextureName + "_nm.png" );
      mat.SpecularGlossinessTexture = new TextureBase( layer.TextureName + "_spec.png" );

      MaterialCache.Add( mat.Name, mat );
      entityNode.Materials.Add( mat );
    }

  }

}
