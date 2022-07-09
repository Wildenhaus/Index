using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Assimp;
using H2AIndex.Common.Extensions;
using H2AIndex.ViewModels;
using Saber3D.Data;
using Saber3D.Data.Geometry;
using Saber3D.Data.Materials;
using Saber3D.Serializers.Geometry;

namespace H2AIndex.Tools
{

  public class SceneExporter
  {

    #region Data Members

    private BinaryReader _reader;
    private S3DGeometryGraph _graph;

    private Scene _scene;
    private Dictionary<short, Node> _nodes;

    #endregion

    #region Properties

    public Scene Scene => _scene;

    public IReadOnlyDictionary<short, Node> Nodes => _nodes;

    public ProgressViewModel Progress { get; }

    #endregion

    #region Constructor

    private SceneExporter( string name, S3DGeometryGraph graph, BinaryReader reader, ProgressViewModel progress )
    {
      _reader = reader;
      _graph = graph;
      Progress = progress;

      Progress.TotalUnits = graph.Objects.Count;
      Progress.CompletedUnits = 0;
      Progress.UnitName = "objects converted";
      Progress.IsIndeterminate = false;

      _scene = new Scene();
      _scene.RootNode = new Node( name );

      _scene.Metadata.Add( "UnitScaleFactor", new Metadata.Entry( MetaDataType.Double, 1d ) );
      _scene.Metadata.Add( "OriginalUnitScaleFactor", new Metadata.Entry( MetaDataType.Float, 1f ) );

      _nodes = new Dictionary<short, Node>();

      _scene.Materials.Add( new Material() { Name = "DefaultMaterial" } );
    }

    public static Scene CreateScene( string name, S3DGeometryGraph graph, BinaryReader reader, ProgressViewModel progress )
    {
      var exporter = new SceneExporter( name, graph, reader, progress );

      exporter.AddObject( graph.RootObject );

      return exporter.Scene;
    }

    #endregion

    #region Public Methods

    public void AddObject( S3DObject obj, Node parentNode = null )
    {
      if ( parentNode is null )
        parentNode = _scene.RootNode;

      var objectNode = new Node( obj.GetName(), parentNode );
      parentNode.Children.Add( objectNode );
      _nodes.Add( obj.Id, objectNode );

      objectNode.Transform = obj.MatrixModel.ToAssimp();

      if ( obj.SubMeshes.Any() )
      {
        if ( obj.GetName().Contains( "shield" ) )
          return;

        AddMeshData( obj, objectNode );
        Progress.CompletedUnits++;
        return;
      }
      else
      {
        Progress.CompletedUnits++;
        foreach ( var childObject in obj.EnumerateChildren() )
          AddObject( childObject, objectNode );
      }

    }

    #endregion

    #region Private Methods

    private void AddMeshData( S3DObject obj, Node objectNode )
    {
      foreach ( var submesh in obj.SubMeshes )
      {
        var mesh = MeshBuilder.Build( _scene, obj, submesh, _reader );
        _scene.Meshes.Add( mesh );
        objectNode.MeshIndices.Add( _scene.Meshes.Count - 1 );
      }
    }

    #endregion

    internal class MeshBuilder
    {

      private Scene _scene;
      private S3DObject _obj;
      private S3DGeometrySubMesh _submesh;
      private S3DGeometryGraph _graph;
      private BinaryReader _reader;

      private Mesh _mesh;

      private Dictionary<short, Bone> _boneLookup;
      private Dictionary<int, int> _vertexLookup;

      private MeshBuilder( Scene scene, S3DObject obj, S3DGeometrySubMesh submesh, BinaryReader reader )
      {
        _scene = scene;
        _obj = obj;
        _submesh = submesh;

        _graph = obj.GeometryGraph;
        _reader = reader;

        _boneLookup = new Dictionary<short, Bone>();
        _vertexLookup = new Dictionary<int, int>();
      }

      public static Mesh Build( Scene scene, S3DObject obj, S3DGeometrySubMesh submesh, BinaryReader reader )
      {
        var builder = new MeshBuilder( scene, obj, submesh, reader );
        return builder.Build();
      }

      private Mesh Build()
      {
        _mesh = new Mesh( _obj.GetName(), PrimitiveType.Triangle );

        var meshData = _graph.Meshes[ ( int ) _submesh.MeshId ];
        foreach ( var meshBuffer in meshData.Buffers )
        {
          var buffer = _graph.Buffers[ ( int ) meshBuffer.BufferId ];

          switch ( buffer.ElementType )
          {
            case S3DGeometryElementType.Vertex:
              AddVertices( buffer, meshBuffer );
              break;
            case S3DGeometryElementType.Face:
              AddFaces( buffer, meshBuffer );
              break;
            case S3DGeometryElementType.Interleaved:
              AddInterleavedData( buffer, meshBuffer );
              break;
            default:
              break;
          }
        }

        if ( _submesh.Material != null )
          AddMaterial( _submesh.Material );

        return _mesh;
      }

      private void AddFaces( S3DGeometryBuffer buffer, S3DGeometryMeshBuffer meshBuffer )
      {
        var offset = _submesh.BufferInfo.FaceOffset;
        var startIndex = offset + meshBuffer.SubBufferOffset / buffer.ElementSize;
        var endIndex = startIndex + _submesh.BufferInfo.FaceCount;

        var serializer = new S3DFaceSerializer( buffer );
        foreach ( var face in serializer.DeserializeRange( _reader, ( int ) startIndex, ( int ) endIndex ) )
        {
          var assimpFace = new Face();
          assimpFace.Indices.Add( _vertexLookup[ face[ 0 ] ] );
          assimpFace.Indices.Add( _vertexLookup[ face[ 1 ] ] );
          assimpFace.Indices.Add( _vertexLookup[ face[ 2 ] ] );

          _mesh.Faces.Add( assimpFace );
        }
      }

      private void AddVertices( S3DGeometryBuffer buffer, S3DGeometryMeshBuffer meshBuffer )
      {
        var offset = _submesh.BufferInfo.VertexOffset;
        var startIndex = offset + meshBuffer.SubBufferOffset / buffer.ElementSize;
        var endIndex = startIndex + _submesh.BufferInfo.VertexCount;

        var scale = new Vector3D( 1, 1, 1 );
        if ( _submesh.Scale.HasValue )
          scale = _submesh.Scale.Value.ToAssimp();

        var pos = new Vector3D( 0, 0, 0 );
        if ( _submesh.Position.HasValue )
          pos = _submesh.Position.Value.ToAssimp();

        var serializer = new S3DVertexSerializer( buffer );
        foreach ( var vertex in serializer.DeserializeRange( _reader, ( int ) startIndex, ( int ) endIndex ) )
        {
          _mesh.Vertices.Add( vertex.Position.ToAssimp3D() * scale + pos );

          if ( vertex.Normal.HasValue )
            _mesh.Normals.Add( vertex.Normal.Value.ToAssimp3D() );

          if ( vertex is S3DVertexSkinned skinnedVertex )
            AddVertexSkinningData( skinnedVertex );
          else
            AddBoneWeight( _obj.ParentId, 1 );

          _vertexLookup.Add( offset++, _vertexLookup.Count );
        }
      }

      private void AddVertexSkinningData( S3DVertexSkinned skinnedVertex )
      {
        var boneIds = _submesh.BoneIds;
        var set = new HashSet<byte>();

        if ( skinnedVertex.Weight1.HasValue && set.Add( skinnedVertex.Index1 ) )
          AddBoneWeight( boneIds[ skinnedVertex.Index1 ], skinnedVertex.Weight1.Value );
        if ( skinnedVertex.Weight2.HasValue && set.Add( skinnedVertex.Index1 ) )
          AddBoneWeight( boneIds[ skinnedVertex.Index2 ], skinnedVertex.Weight2.Value );
        if ( skinnedVertex.Weight3.HasValue && set.Add( skinnedVertex.Index1 ) )
          AddBoneWeight( boneIds[ skinnedVertex.Index3 ], skinnedVertex.Weight3.Value );
        if ( skinnedVertex.Weight4.HasValue && set.Add( skinnedVertex.Index1 ) )
          AddBoneWeight( boneIds[ skinnedVertex.Index4 ], skinnedVertex.Weight4.Value );
      }

      private void AddBoneWeight( short boneObjectId, float weight )
      {
        if ( boneObjectId == -1 )
          return;

        if ( !_boneLookup.TryGetValue( boneObjectId, out var bone ) )
        {
          var boneObject = _graph.Objects[ boneObjectId ];

          System.Numerics.Matrix4x4.Invert( boneObject.MatrixLT, out var invMatrix );

          bone = new Bone
          {
            Name = boneObject.GetName(),
            OffsetMatrix = invMatrix.ToAssimp()
          };

          _mesh.Bones.Add( bone );
          _boneLookup.Add( boneObjectId, bone );
        }

        bone.VertexWeights.Add( new VertexWeight( _mesh.Vertices.Count - 1, weight ) );
      }

      private void AddInterleavedData( S3DGeometryBuffer buffer, S3DGeometryMeshBuffer meshBuffer )
      {
        var offset = _submesh.BufferInfo.VertexOffset;
        var startIndex = offset + ( meshBuffer.SubBufferOffset / buffer.ElementSize );
        var endIndex = startIndex + _submesh.BufferInfo.VertexCount;

        var serializer = new S3DInterleavedDataSerializer( buffer );
        foreach ( var data in serializer.DeserializeRange( _reader, ( int ) startIndex, ( int ) endIndex ) )
        {
          if ( data.UV0.HasValue ) AddVertexUV( 0, data.UV0.Value );
          if ( data.UV1.HasValue ) AddVertexUV( 1, data.UV1.Value );
          if ( data.UV2.HasValue ) AddVertexUV( 2, data.UV2.Value );
          if ( data.UV3.HasValue ) AddVertexUV( 3, data.UV3.Value );
          if ( data.UV4.HasValue ) AddVertexUV( 4, data.UV4.Value );

          // TODO
          /* Assimp only allows 1 Tangent channel.
           * Multiple tangent channels seem to only occur for level geometry.
           * See AddVertexTangent() for more info.
           */
          if ( data.Tangent0.HasValue ) AddVertexTangent( 0, data.Tangent0.Value );
          //if ( data.Tangent1.HasValue ) System.Diagnostics.Debugger.Break();
          //if ( data.Tangent2.HasValue ) System.Diagnostics.Debugger.Break();
          //if ( data.Tangent3.HasValue ) System.Diagnostics.Debugger.Break();
          //if ( data.Tangent4.HasValue ) System.Diagnostics.Debugger.Break();
        }
      }

      private void AddVertexTangent( byte tangentChannel, Vector4 tangentVector )
      {
        /* It seems that Assimp only supports 1 tangent channel.
         * In the vertex buffers, there can be up to 4 tangents.
         * Not sure if I'm just not applying this in the right place
         * or if there's more trickery involved.
         * 
         * Just setting the main tangent channel to the first tangent in the buffer for now.
         */
        _mesh.Tangents.Add( tangentVector.ToAssimp3D() );
      }

      private void AddVertexUV( byte uvChannel, Vector4 uvVector )
      {
        if ( !_submesh.UvScaling.TryGetValue( uvChannel, out var scaleFactor ) )
          scaleFactor = 1;

        var scaleVector = new Vector3D( scaleFactor );
        var scaledUvVector = uvVector.ToAssimp3D() * scaleVector;

        _mesh.TextureCoordinateChannels[ uvChannel ].Add( scaledUvVector );

        /* This is a bit confusing, but this property denotes the size of the UV element.
         * E.g. setting it to 2 means there is a U and a V.
         * I don't know how 4D UVs work, but if we ever add support for them, we'd need to
         * adjust this accordingly.
         */
        _mesh.UVComponentCount[ uvChannel ] = 2;
      }

      private void AddMaterial( S3DMaterial material )
      {
        var materialTexName = material.ShadingMaterialTexture;

        for ( var i = 0; i < _scene.Materials.Count; i++ )
        {
          var sceneMat = _scene.Materials[ i ];
          if ( sceneMat.Name == materialTexName )
          {
            _mesh.MaterialIndex = i;
            return;
          }
        }

        var mat = new Material { Name = materialTexName };
        mat.TextureDiffuse = new TextureSlot { FilePath = materialTexName };
        mat.TextureNormal = new TextureSlot { FilePath = $"{materialTexName}_nm" };
        mat.TextureSpecular = new TextureSlot { FilePath = $"{materialTexName}_spec" };

        _scene.Materials.Add( mat );
        _mesh.MaterialIndex = _scene.Materials.Count - 1;
      }

    }

  }

}
