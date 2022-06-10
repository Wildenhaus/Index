using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Aspose.ThreeD;
using Aspose.ThreeD.Entities;
using Aspose.ThreeD.Utilities;
using Saber3D.Data;
using Saber3D.Data.Geometry;

namespace Testbed
{

  public class ArmatureExportTest
  {

    private S3DGeometryGraph _graph;

    private Scene _scene;
    private Node _armatureNode;

    public ArmatureExportTest( S3DGeometryGraph graph )
    {
      _graph = graph;
      _scene = new Scene();

      Init();
    }

    public void Save( Stream stream )
    {
      _scene.Save( stream, FileFormat.FBX7700Binary );
    }

    private void Init()
    {
      var rootObj = _graph.Objects[ _graph.RootNodeIndex ];

      foreach ( var child in rootObj.EnumerateChildren( _graph ) )
        AddObject( child );
    }

    private void AddObject( S3DObject obj, Node parentNode = null )
    {
      if ( parentNode is null )
        parentNode = _scene.RootNode;

      var objNode = parentNode.CreateChildNode( obj.Name );

      if ( obj.GeomData is null )
        AddBone( obj, objNode );
      else
        AddMesh( obj, objNode );

      foreach ( var childObj in obj.EnumerateChildren( _graph ) )
        AddObject( childObj, objNode );
    }

    private void AddBone( S3DObject obj, Node node )
    {
      if ( _graph.SubMeshes.Any( x => x.NodeId == obj.Id ) )
        return;

      var bone = new Skeleton( obj.Name ) { Type = SkeletonType.Bone };
      node.AddEntity( bone );

      /* Objects have two matrices: MatrixLT and MatrixModel.
       * MatrixLT: Local transform?
       * MatrixModel: Global transform?
       * 
       * Setting the bone's transform matrix to MatrixModel seems /somewhat/ correct
       * from the waist up. There are lots issues with it though.
       */
      var matr = obj.MatrixLT;
      var matr2 = obj.MatrixModel;

      /* Vectors/Matrices are different types between System.Numerics and the Aspose3D library.
       * I'm keeping everything as System.Numerics until it's applied to the model, as System.Numerics
       * lets you do math on it.
       * 
       * Aspose: Matrix4
       * System.Numerics: Matrix4x4
       * 
       * I've added extension methods so we can easily convert between the two.
       */
      node.Transform.TransformMatrix = matr2.ToMatrix4();
    }

    private void AddMesh( S3DObject obj, Node node )
    {
      var mesh = MeshBuilder.Create( obj, _graph );
      node.AddEntity( mesh );
      node.Transform.TransformMatrix = obj.MatrixModel.ToMatrix4();
    }

  }

  internal class MeshBuilder
  {

    private S3DObject _obj;
    private S3DGeometryGraph _graph;

    private Mesh _mesh;

    private Dictionary<int, int> _faceMap;

    private VertexElementNormal _normals;
    private VertexElementTangent[] _tangents;
    private VertexElementUV[] _uvs;

    private MeshBuilder( S3DObject obj, S3DGeometryGraph graph )
    {
      _obj = obj;
      _graph = graph;

      _mesh = new Mesh( obj.Name );
      _faceMap = new Dictionary<int, int>();

      _tangents = new VertexElementTangent[ 5 ];
      _uvs = new VertexElementUV[ 5 ];
    }

    public static Mesh Create( S3DObject obj, S3DGeometryGraph graph )
    {
      var builder = new MeshBuilder( obj, graph );
      return builder.Build();
    }

    private Mesh Build()
    {
      var submeshes = _graph.SubMeshes.Where( x => x.NodeId == _obj.Id )
        .OrderBy( x => x.BufferInfo.VertexOffset );

      foreach ( var submesh in submeshes )
        AddSubMesh( submesh );

      return _mesh;
    }

    private void AddSubMesh( S3DGeometrySubMesh submesh )
    {
      var meshData = _graph.Meshes[ ( int ) submesh.MeshId ];
      foreach ( var meshBuffer in meshData.Buffers )
      {
        var buffer = _graph.Buffers[ ( int ) meshBuffer.BufferId ];

        switch ( buffer.ElementType )
        {
          case S3DGeometryElementType.Vertex:
            AddVertices( buffer, meshBuffer, submesh );
            break;
          case S3DGeometryElementType.Face:
            AddFaces( buffer, meshBuffer, submesh );
            break;
          case S3DGeometryElementType.Interleaved:
            AddInterleavedData( buffer, meshBuffer, submesh );
            break;
        }

      }
    }

    private void AddInterleavedData( S3DGeometryBuffer buffer, S3DGeometryMeshBuffer meshBuffer, S3DGeometrySubMesh submesh )
    {
      var offset = submesh.BufferInfo.VertexOffset;
      var startIndex = offset + ( meshBuffer.SubBufferOffset / buffer.ElementSize );
      var endIndex = startIndex + submesh.BufferInfo.VertexCount;

      for ( var i = startIndex; i < endIndex; i++ )
      {
        var data = buffer.Elements[ i ] as S3DInterleavedData;

        if ( data.UV0.HasValue ) AddVertexUV( 0, data.UV0.Value );
        if ( data.UV1.HasValue ) AddVertexUV( 1, data.UV1.Value );
        if ( data.UV2.HasValue ) AddVertexUV( 2, data.UV2.Value );
        if ( data.UV3.HasValue ) AddVertexUV( 3, data.UV3.Value );
        if ( data.UV4.HasValue ) AddVertexUV( 4, data.UV4.Value );

        if ( data.Tangent0.HasValue ) AddVertexTangent( 0, data.Tangent0.Value );
        if ( data.Tangent1.HasValue ) AddVertexTangent( 1, data.Tangent1.Value );
        if ( data.Tangent2.HasValue ) AddVertexTangent( 2, data.Tangent2.Value );
        if ( data.Tangent3.HasValue ) AddVertexTangent( 3, data.Tangent3.Value );
        if ( data.Tangent4.HasValue ) AddVertexTangent( 4, data.Tangent4.Value );
      }
    }

    private void AddFaces( S3DGeometryBuffer buffer, S3DGeometryMeshBuffer meshBuffer, S3DGeometrySubMesh submesh )
    {
      var offset = submesh.BufferInfo.FaceOffset;
      var startIndex = offset + ( meshBuffer.SubBufferOffset / buffer.ElementSize );
      var endIndex = startIndex + submesh.BufferInfo.FaceCount;

      for ( var i = startIndex; i < endIndex; i++ )
      {
        var face = buffer.Elements[ i ] as S3DFace;

        var vertA = _faceMap[ face[ 0 ] ];
        var vertB = _faceMap[ face[ 1 ] ];
        var vertC = _faceMap[ face[ 2 ] ];

        _mesh.CreatePolygon( vertA, vertB, vertC );
      }
    }

    private void AddVertices( S3DGeometryBuffer buffer, S3DGeometryMeshBuffer meshBuffer, S3DGeometrySubMesh submesh )
    {
      var offset = submesh.BufferInfo.VertexOffset;
      var startIndex = offset + ( meshBuffer.SubBufferOffset / buffer.ElementSize );
      var endIndex = startIndex + submesh.BufferInfo.VertexCount;

      var scale = new float[] { 1, 1, 1 };
      if ( submesh.Scale.HasValue )
      {
        scale[ 0 ] = submesh.Scale.Value.X;
        scale[ 1 ] = submesh.Scale.Value.Y;
        scale[ 2 ] = submesh.Scale.Value.Z;
      }

      var pos = new float[] { 0, 0, 0 };
      if ( submesh.Position.HasValue )
      {
        pos[ 0 ] = submesh.Position.Value.X;
        pos[ 1 ] = submesh.Position.Value.Y;
        pos[ 2 ] = submesh.Position.Value.Z;
      }

      for ( var i = startIndex; i < endIndex; i++ )
      {
        var vertex = buffer.Elements[ i ] as S3DVertex;
        var vertexPos = new Aspose.ThreeD.Utilities.Vector4(
          vertex.X * scale[ 0 ] + pos[ 0 ],
          vertex.Y * scale[ 1 ] + pos[ 1 ],
          vertex.Z * scale[ 2 ] + pos[ 2 ],
          1 );

        _mesh.ControlPoints.Add( vertexPos );
        _faceMap.Add( ( int ) offset++, _faceMap.Count );

        if ( vertex.Normal.HasValue )
        {
          if ( _normals is null )
            _normals = _mesh.CreateElement( VertexElementType.Normal, MappingMode.ControlPoint, ReferenceMode.Direct ) as VertexElementNormal;

          var normal = vertex.Normal.Value;
          var normalVec = new Aspose.ThreeD.Utilities.Vector4( normal.X, normal.Y, normal.Z );
          _normals.Data.Add( normalVec );
        }
      }
    }

    private void AddVertexTangent( int uvIndex, System.Numerics.Vector4 tangentVec )
    {
      if ( _tangents[ uvIndex ] is null )
        _tangents[ uvIndex ] = _mesh.CreateElement( VertexElementType.Tangent, MappingMode.ControlPoint, ReferenceMode.Direct ) as VertexElementTangent;

      var vec = new Aspose.ThreeD.Utilities.Vector4( tangentVec.X, tangentVec.Y, tangentVec.Z, tangentVec.W );
      _tangents[ uvIndex ].Data.Add( vec );
    }

    private void AddVertexUV( int uvIndex, System.Numerics.Vector4 uvVector )
    {
      if ( _uvs[ uvIndex ] is null )
        _uvs[ uvIndex ] = _mesh.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );

      var uvVec = new Aspose.ThreeD.Utilities.Vector4( uvVector.X, uvVector.Y, 0, 0 );
      _uvs[ uvIndex ].Data.Add( uvVec );
    }

  }

  public static class S3DObjectEx
  {

    public static IEnumerable<S3DObject> EnumerateChildren( this S3DObject obj, S3DGeometryGraph graph )
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

    public static Matrix4 ToMatrix4( this System.Numerics.Matrix4x4 matr )
    {
      return new Matrix4(
        matr.M11, matr.M12, matr.M13, matr.M14,
        matr.M21, matr.M22, matr.M23, matr.M24,
        matr.M31, matr.M32, matr.M33, matr.M34,
        matr.M41, matr.M42, matr.M43, matr.M44
        );
    }

    public static Matrix4x4 ToMatrix4x4( this Matrix4 matr )
    {
      return new Matrix4x4(
        ( float ) matr.m00, ( float ) matr.m01, ( float ) matr.m02, ( float ) matr.m03,
        ( float ) matr.m10, ( float ) matr.m11, ( float ) matr.m12, ( float ) matr.m13,
        ( float ) matr.m20, ( float ) matr.m21, ( float ) matr.m22, ( float ) matr.m23,
        ( float ) matr.m30, ( float ) matr.m31, ( float ) matr.m32, ( float ) matr.m33
        );
    }

  }

}
