using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assimp;
using Saber3D.Data;
using Saber3D.Data.Geometry;
using Saber3D.Serializers.Geometry;

namespace Testbed
{

  public class SceneExporter
  {

    #region Data Members

    private BinaryReader _reader;

    private Scene _scene;
    private Dictionary<short, Node> _nodes;

    #endregion

    #region Properties

    public Scene Scene => _scene;

    public IReadOnlyDictionary<short, Node> Nodes => _nodes;

    #endregion

    #region Constructor

    private SceneExporter( string name, BinaryReader reader )
    {
      _reader = reader;

      _scene = new Scene();
      _scene.RootNode = new Node( name );

      _scene.Metadata.Add( "UnitScaleFactor", new Metadata.Entry( MetaDataType.Double, 1d ) );
      _scene.Metadata.Add( "OriginalUnitScaleFactor", new Metadata.Entry( MetaDataType.Float, 1f ) );

      _nodes = new Dictionary<short, Node>();

      _scene.Materials.Add( new Material() { Name = "DefaultMaterial" } );
    }

    public static Scene CreateScene( string name, S3DGeometryGraph graph, BinaryReader reader )
    {
      var exporter = new SceneExporter( name, reader );

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
        AddMeshData( obj, objectNode );
        return;
      }
      else
      {
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
        var mesh = MeshBuilder.Build( obj, submesh, _reader );
        _scene.Meshes.Add( mesh );
        objectNode.MeshIndices.Add( _scene.Meshes.Count - 1 );
      }
    }

    #endregion

    internal class MeshBuilder
    {

      private S3DObject _obj;
      private S3DGeometrySubMesh _submesh;
      private S3DGeometryGraph _graph;
      private BinaryReader _reader;

      private Mesh _mesh;

      private Dictionary<ushort, Bone> _boneLookup;
      private Dictionary<int, int> _vertexLookup;

      private MeshBuilder( S3DObject obj, S3DGeometrySubMesh submesh, BinaryReader reader )
      {
        _obj = obj;
        _submesh = submesh;

        _graph = obj.GeometryGraph;
        _reader = reader;

        _boneLookup = new Dictionary<ushort, Bone>();
        _vertexLookup = new Dictionary<int, int>();
      }

      public static Mesh Build( S3DObject obj, S3DGeometrySubMesh submesh, BinaryReader reader )
      {
        var builder = new MeshBuilder( obj, submesh, reader );
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
              //AddInterleavedData( buffer, meshBuffer, submesh );
              break;
            default:
              break;
          }
        }

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
            AddWeight( ( ushort ) _obj.ParentId, 1 );

          _vertexLookup.Add( offset++, _vertexLookup.Count );
        }
      }

      private void AddVertexSkinningData( S3DVertexSkinned skinnedVertex )
      {
        var boneIds = _submesh.BoneIds;
        var set = new HashSet<byte>();

        if ( skinnedVertex.Weight1.HasValue && set.Add( skinnedVertex.Index1 ) )
          AddWeight( boneIds[ skinnedVertex.Index1 ], skinnedVertex.Weight1.Value );
        if ( skinnedVertex.Weight2.HasValue && set.Add( skinnedVertex.Index1 ) )
          AddWeight( boneIds[ skinnedVertex.Index2 ], skinnedVertex.Weight2.Value );
        if ( skinnedVertex.Weight3.HasValue && set.Add( skinnedVertex.Index1 ) )
          AddWeight( boneIds[ skinnedVertex.Index3 ], skinnedVertex.Weight3.Value );
        if ( skinnedVertex.Weight4.HasValue && set.Add( skinnedVertex.Index1 ) )
          AddWeight( boneIds[ skinnedVertex.Index4 ], skinnedVertex.Weight4.Value );
      }

      private void AddWeight( ushort boneObjectId, float weight )
      {
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

    }

  }

}
