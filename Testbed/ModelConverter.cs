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

  public static class ModelConverter
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

      _visitedNodes = new HashSet<short>();
      foreach ( var obj in graph.Objects.Skip( graph.RootNodeIndex ) )
        AddObject( scene, obj, graph );

      var outStream = new MemoryStream();
      scene.Save( outStream, FileFormat.FBX7700Binary );

      outStream.Position = 0;
      return outStream;
    }

    private static HashSet<short> _visitedNodes;
    public static Stream ConvertScn( Stream inStream )
    {
      MaterialCache.Clear();

      var reader = new BinaryReader( inStream );
      var scn = new S3DSceneSerializer().Deserialize( reader );
      var graph = scn.GeometryGraph;

      var scene = new Scene();

      _visitedNodes = new HashSet<short>();
      foreach ( var obj in graph.Objects.Skip( graph.RootNodeIndex ) )
        AddObject( scene, obj, graph );

      var outStream = new MemoryStream();
      scene.Save( outStream, FileFormat.FBX7700Binary );

      outStream.Position = 0;
      return outStream;
    }

    private static void AddObject( Scene scene, S3DObject obj, S3DGeometryGraph graph )
    {
      if ( obj.Id < graph.RootNodeIndex )
        return;
      if ( !graph.SubMeshes.Any( x => x.NodeId == obj.Id ) )
        return;
      if ( !_visitedNodes.Add( obj.Id ) )
        return;

      var node = new Node( obj.Name );
      scene.RootNode.AddChildNode( node );
      AddObjectMesh( node, graph.SubMeshes.Where( x => x.NodeId == obj.Id ), graph );

      // Skip LODs
      foreach ( var child in EnumerateObjectChildren( obj, graph ) )
        _visitedNodes.Add( child.Id );
    }

    private static void AddObjectMesh( Node node, IEnumerable<S3DGeometrySubMesh> submeshes, S3DGeometryGraph graph )
    {
      var mesh = new Mesh();
      node.AddEntity( mesh );
      var obj = graph.Objects[ submeshes.First().NodeId ];

      var buffers = graph.Buffers;

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

      var startVert = submeshes.Min( x => x.BufferInfo.VertexOffset );

      foreach ( var submeshData in submeshes )
      {
        var meshData = graph.Meshes.First( x => x.Id == submeshData.MeshId );
        var meshBuffers = meshData.Buffers;

        var scale = new float[] { 1f, 1f, 1f };
        if ( submeshData.Scale.HasValue )
        {
          scale[ 0 ] = submeshData.Scale.Value.X;
          scale[ 1 ] = submeshData.Scale.Value.Y;
          scale[ 2 ] = submeshData.Scale.Value.Z;
        }

        foreach ( var meshBuffer in meshBuffers )
        {
          var buffer = buffers[ ( int ) meshBuffer.BufferId ];
          var startOffset = buffer.StartOffset + meshBuffer.SubBufferOffset;
          var submeshBufferInfo = submeshData.BufferInfo;

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
                var vert = new Vector4( element.X * scale[ 0 ], element.Y * scale[ 1 ], element.Z * scale[ 2 ] );

                mesh.ControlPoints.Add( vert );

                if ( element.Normal.HasValue )
                {
                  if ( vertexNormals is null )
                    vertexNormals = mesh.CreateElement( VertexElementType.Normal, MappingMode.ControlPoint, ReferenceMode.Direct ) as VertexElementNormal;

                  var norm = new Vector4( element.Normal.Value.X, element.Normal.Value.Y, element.Normal.Value.Z, 0 );
                  vertexNormals.Data.Add( norm );
                }
                faceMap.Add( ( int ) offset, mesh.ControlPoints.Count - 1 );
                offset++;
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

                mesh.CreatePolygon( faceMap[ element[ 0 ] ], faceMap[ element[ 1 ] ], faceMap[ element[ 2 ] ] );
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
                    tan0 = mesh.CreateElement( VertexElementType.Tangent, MappingMode.ControlPoint, ReferenceMode.Direct ) as VertexElementTangent;
                  var t = element.Tangent0.Value;
                  tan0.Data.Add( new Vector4( t.X, t.Y, t.Z, t.W ) );
                }
                if ( element.Tangent1.HasValue )
                {
                  if ( tan1 is null )
                    tan1 = mesh.CreateElement( VertexElementType.Tangent, MappingMode.ControlPoint, ReferenceMode.Direct ) as VertexElementTangent;
                  var t = element.Tangent1.Value;
                  tan1.Data.Add( new Vector4( t.X, t.Y, t.Z, t.W ) );
                }
                if ( element.Tangent2.HasValue )
                {
                  if ( tan2 is null )
                    tan2 = mesh.CreateElement( VertexElementType.Tangent, MappingMode.ControlPoint, ReferenceMode.Direct ) as VertexElementTangent;
                  var t = element.Tangent2.Value;
                  tan2.Data.Add( new Vector4( t.X, t.Y, t.Z, t.W ) );
                }
                if ( element.Tangent3.HasValue )
                {
                  if ( tan3 is null )
                    tan3 = mesh.CreateElement( VertexElementType.Tangent, MappingMode.ControlPoint, ReferenceMode.Direct ) as VertexElementTangent;
                  var t = element.Tangent3.Value;
                  tan3.Data.Add( new Vector4( t.X, t.Y, t.Z, t.W ) );
                }
                if ( element.Tangent4.HasValue )
                {
                  if ( tan4 is null )
                    tan4 = mesh.CreateElement( VertexElementType.Tangent, MappingMode.ControlPoint, ReferenceMode.Direct ) as VertexElementTangent;
                  var t = element.Tangent4.Value;
                  tan4.Data.Add( new Vector4( t.X, t.Y, t.Z, t.W ) );
                }

                if ( element.UV0.HasValue )
                {
                  if ( uv0 is null )
                    uv0 = mesh.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
                  var uv = element.UV0.Value;
                  uv0.Data.Add( new Vector4( uv.X, uv.Y, uv.Z, uv.W ) );
                }
                if ( element.UV1.HasValue )
                {
                  if ( uv1 is null )
                    uv1 = mesh.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
                  var uv = element.UV1.Value;
                  uv1.Data.Add( new Vector4( uv.X, uv.Y, uv.Z, uv.W ) );
                }
                if ( element.UV2.HasValue )
                {
                  if ( uv2 is null )
                    uv2 = mesh.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
                  var uv = element.UV2.Value;
                  uv2.Data.Add( new Vector4( uv.X, uv.Y, uv.Z, uv.W ) );
                }
                if ( element.UV3.HasValue )
                {
                  if ( uv3 is null )
                    uv3 = mesh.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
                  var uv = element.UV3.Value;
                  uv3.Data.Add( new Vector4( uv.X, uv.Y, uv.Z, uv.W ) );
                }
                if ( element.UV4.HasValue )
                {
                  if ( uv4 is null )
                    uv4 = mesh.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
                  var uv = element.UV4.Value;
                  uv4.Data.Add( new Vector4( uv.X, uv.Y, uv.Z, uv.W ) );
                }
              }

            }
            break;
          }
        }

        ApplySubMeshMaterials( node, submeshData );
      }
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
      mat.NormalTexture = new Texture( layer.TextureName + "_nm.png" );
      mat.SpecularGlossinessTexture = new Texture( layer.TextureName + "_spec.png" );

      MaterialCache.Add( mat.Name, mat );
      entityNode.Materials.Add( mat );
    }

    private static IEnumerable<S3DObject> EnumerateObjectChildren( S3DObject obj, S3DGeometryGraph graph )
    {
      var visited = new HashSet<short>();

      var currentId = obj.ChildId;
      while ( visited.Add( currentId ) )
      {
        if ( currentId < 0 )
          break;

        yield return graph.Objects[ currentId ];
        currentId = graph.Objects[ currentId ].NextId;
      }
    }

  }

}
