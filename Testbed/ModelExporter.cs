using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Aspose.ThreeD;
using Aspose.ThreeD.Deformers;
using Aspose.ThreeD.Entities;
using Aspose.ThreeD.Shading;
using Aspose.ThreeD.Utilities;
using Saber3D.Data;
using Saber3D.Data.Geometry;
using Saber3D.Serializers.Geometry;

namespace Testbed
{

  public class ModelExporter
  {

    private S3DGeometryGraph _graph;

    private Scene _scene;
    private Armature _armature;
    private BinaryReader _reader;

    public ModelExporter( S3DGeometryGraph graph, BinaryReader reader )
    {
      _graph = graph;
      _reader = reader;

      _scene = new Scene();

      Init();
    }

    public void Save( Stream stream )
    {
      _scene.Save( stream, FileFormat.FBX7200Binary );
    }

    private void Init()
    {
      _armature = Armature.Create( _graph );
      _scene.RootNode.AddChildNode( _armature.ArmatureNode );
      _scene.Poses.Add( _armature.BindPose );

      var rootObj = _graph.Objects[ _graph.RootNodeIndex ];
      foreach ( var child in rootObj.EnumerateChildren( _graph ) )
        AddObject( child );
    }

    private void AddObject( S3DObject obj, Node parentNode = null )
    {
      if ( parentNode is null )
        parentNode = _scene.RootNode;


      if ( obj.GeomData is not null )
      {
        var objNode = parentNode.CreateChildNode( obj.Name ?? obj.ReadName );
        AddMesh( obj, objNode );
      }
      else
      {
        foreach ( var childObj in obj.EnumerateChildren( _graph ) )
          AddObject( childObj, parentNode );
      }
    }

    private void AddMesh( S3DObject obj, Node node )
    {
      MeshBuilder.Create( _reader, node, _armature, obj, _graph );
    }

  }

  internal class Armature
  {

    private S3DGeometryGraph _graph;

    private Node _armatureNode;
    private Dictionary<int, Bone> _bones;
    private Pose _bindPose;

    public Node ArmatureNode
    {
      get => _armatureNode;
    }

    public IReadOnlyDictionary<int, Bone> Bones
    {
      get => _bones;
    }

    public Pose BindPose => _bindPose;

    private Armature( S3DGeometryGraph graph )
    {
      _graph = graph;
      _bones = new Dictionary<int, Bone>();
    }

    public static Armature Create( S3DGeometryGraph graph )
    {
      var armature = new Armature( graph );
      armature.Init();

      return armature;
    }

    private void Init()
    {
      var armatureSkeleton = new Skeleton( "Armature" ) { Type = SkeletonType.Skeleton };

      _armatureNode = new Node( "Armature" );
      _armatureNode.AddEntity( armatureSkeleton );

      _bindPose = new Pose( "Default" ) { PoseType = PoseType.BindPose };

      var rootObj = _graph.Objects[ _graph.RootNodeIndex ];
      foreach ( var childObj in rootObj.EnumerateChildren( _graph ) )
        AddBone( childObj, _armatureNode );
    }

    private void AddBone( S3DObject obj, Node parentNode )
    {
      if ( obj.GeomData is not null )
        return;

      // Create bone
      var skelBone = new Skeleton( obj.Name ?? obj.ReadName ) { Type = SkeletonType.Bone };
      var boneNode = parentNode.CreateChildNode( obj.Name ?? obj.ReadName, skelBone );

      // Add bone to deformer
      var deformerBone = new Bone( obj.Name ?? obj.ReadName );
      deformerBone.Node = boneNode;
      deformerBone.BoneTransform = obj.MatrixLT.ToMatrix4();
      _bones.Add( obj.Id, deformerBone );

      // Add bone to pose
      var bonePose = new BonePose
      {
        Matrix = obj.MatrixLT.ToMatrix4(),
        Node = boneNode,
      };
      _bindPose.BonePoses.Add( bonePose );

      foreach ( var childObj in obj.EnumerateChildren( _graph ) )
        AddBone( childObj, boneNode );
    }

  }

  internal class MeshBuilder
  {

    private static Dictionary<string, Material> _materialCache = new Dictionary<string, Material>();

    private BinaryReader _reader;

    private Armature _armature;

    private S3DObject _obj;
    private S3DGeometryGraph _graph;

    private Node _node;
    private Mesh _mesh;
    private SkinDeformer _deformer;

    private Dictionary<int, int> _faceMap;

    private VertexElementNormal _normals;
    private VertexElementTangent[] _tangents;
    private VertexElementUV[] _uvs;
    private VertexElementMaterial _materials;

    private Dictionary<string, int> _materialLookup;

    private MeshBuilder( BinaryReader reader, Node node, Armature armature, S3DObject obj, S3DGeometryGraph graph )
    {
      _reader = reader;

      _node = node;
      _armature = armature;
      _obj = obj;
      _graph = graph;

      _mesh = new Mesh( obj.Name ?? obj.ReadName );
      _node.AddEntity( _mesh );

      _faceMap = new Dictionary<int, int>();

      _tangents = new VertexElementTangent[ 5 ];
      _uvs = new VertexElementUV[ 5 ];

      _materialLookup = new Dictionary<string, int>();
    }

    public static Mesh Create( BinaryReader reader, Node node, Armature armature, S3DObject obj, S3DGeometryGraph graph )
    {
      var builder = new MeshBuilder( reader, node, armature, obj, graph );
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
      AddSubMeshMaterial( submesh );

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
          default:
            break;
        }
      }

    }

    private void AddSubMeshMaterial( S3DGeometrySubMesh subMesh )
    {
      if ( subMesh.Material is null )
        return;

      var referenceMat = subMesh.Material;

      if ( !_materialCache.TryGetValue( referenceMat.ShadingMaterialTexture, out var mat ) )
      {
        var newMat = new PbrSpecularMaterial();
        newMat.Name = subMesh.Material.ShadingMaterialTexture;

        newMat.DiffuseTexture = new Texture( referenceMat.ShadingMaterialTexture + ".png" );
        newMat.NormalTexture = new Texture( referenceMat.ShadingMaterialTexture + "_nm.png" );
        newMat.SpecularGlossinessTexture = new TextureBase( referenceMat.ShadingMaterialTexture + "_spec.png" );

        _materialCache.Add( referenceMat.ShadingMaterialTexture, newMat );

        mat = newMat;
      }

      _node.Materials.Add( mat );
    }

    private void AddInterleavedData( S3DGeometryBuffer buffer, S3DGeometryMeshBuffer meshBuffer, S3DGeometrySubMesh submesh )
    {
      var offset = submesh.BufferInfo.VertexOffset;
      var startIndex = offset + ( meshBuffer.SubBufferOffset / buffer.ElementSize );
      var endIndex = startIndex + submesh.BufferInfo.VertexCount;

      var uvScaleLookup = submesh.UvScaling;

      var serializer = new S3DInterleavedDataSerializer( buffer );
      foreach ( var data in serializer.DeserializeRange( _reader, ( int ) startIndex, ( int ) endIndex ) )
      {
        if ( data.UV0.HasValue ) AddVertexUV( 0, data.UV0.Value, uvScaleLookup );
        if ( data.UV1.HasValue ) AddVertexUV( 1, data.UV1.Value, uvScaleLookup );
        if ( data.UV2.HasValue ) AddVertexUV( 2, data.UV2.Value, uvScaleLookup );
        if ( data.UV3.HasValue ) AddVertexUV( 3, data.UV3.Value, uvScaleLookup );
        if ( data.UV4.HasValue ) AddVertexUV( 4, data.UV4.Value, uvScaleLookup );

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

      var serializer = new S3DFaceSerializer( buffer );
      foreach ( var face in serializer.DeserializeRange( _reader, ( int ) startIndex, ( int ) endIndex ) )
      {
        var vertA = _faceMap[ face[ 0 ] ];
        var vertB = _faceMap[ face[ 1 ] ];
        var vertC = _faceMap[ face[ 2 ] ];

        _mesh.CreatePolygon( vertA, vertB, vertC );

        if ( submesh.Material is not null )
        {
          if ( !_materialLookup.TryGetValue( submesh.Material.ShadingMaterialTexture, out var matIdx ) )
          {
            if ( _materials is null )
              _materials = _mesh.CreateElement( VertexElementType.Material, MappingMode.Polygon, ReferenceMode.Index ) as VertexElementMaterial;

            matIdx = _materialLookup.Count;
            _materialLookup.Add( submesh.Material.ShadingMaterialTexture, matIdx );
          }

          _materials.Indices.Add( matIdx );
        }
      }
    }

    private void AddVertices( S3DGeometryBuffer buffer, S3DGeometryMeshBuffer meshBuffer, S3DGeometrySubMesh submesh )
    {
      var offset = submesh.BufferInfo.VertexOffset;
      var startIndex = offset + ( meshBuffer.SubBufferOffset / buffer.ElementSize );
      var endIndex = startIndex + submesh.BufferInfo.VertexCount;

      var obj = _graph.Objects[ submesh.NodeId ];
      SkinDeformer deformer = null;

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

      var serializer = new S3DVertexSerializer( buffer );
      foreach ( var vertex in serializer.DeserializeRange( _reader, ( int ) startIndex, ( int ) endIndex ) )
      {
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

        if ( vertex is S3DVertexSkinned skinnedVertex )
        {
          if ( deformer is null )
          {
            deformer = new SkinDeformer();
            _mesh.Deformers.Add( deformer );

            /* This library doesn't like it when we reuse Bone objects.
             * We need to create a new one for each deformer, or else it
             * messes up the weights.
             * 
             * It's just a case of bad semantics. I can't believe Aspose charges
             * $4,000 for a product with no documentation and bad naming schemes like this.
             */
            foreach ( var boneId in submesh.BoneIds )
            {
              var referenceBone = _armature.Bones[ boneId ];
              var newBone = new Bone( referenceBone.Name )
              {
                BoneTransform = referenceBone.BoneTransform,
                Node = referenceBone.Node
              };

              deformer.Bones.Add( newBone );
            }
          }

          var boneIds = submesh.BoneIds;
          var vertIndex = _mesh.ControlPoints.Count - 1;
          var set = new HashSet<int>();

          if ( skinnedVertex.Weight1.HasValue && set.Add( skinnedVertex.Index1 ) && skinnedVertex.Weight1.Value > 0 )
            deformer.Bones[ skinnedVertex.Index1 ].SetWeight( vertIndex, skinnedVertex.Weight1.Value );

          if ( skinnedVertex.Weight2.HasValue && set.Add( skinnedVertex.Index2 ) && skinnedVertex.Weight2.Value > 0 )
            deformer.Bones[ skinnedVertex.Index2 ].SetWeight( vertIndex, skinnedVertex.Weight2.Value );

          if ( skinnedVertex.Weight3.HasValue && set.Add( skinnedVertex.Index3 ) && skinnedVertex.Weight3.Value > 0 )
            deformer.Bones[ skinnedVertex.Index3 ].SetWeight( vertIndex, skinnedVertex.Weight3.Value );

          if ( skinnedVertex.Weight4.HasValue && set.Add( skinnedVertex.Index4 ) && skinnedVertex.Weight4.Value > 0 )
            deformer.Bones[ skinnedVertex.Index4 ].SetWeight( vertIndex, skinnedVertex.Weight4.Value );
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

    private void AddVertexUV( int uvIndex, System.Numerics.Vector4 uvVector, Dictionary<byte, short> scaleLookup )
    {
      if ( !scaleLookup.TryGetValue( ( byte ) uvIndex, out var scaleFactor ) )
        scaleFactor = 1;

      if ( _uvs[ uvIndex ] is null )
        _uvs[ uvIndex ] = _mesh.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );

      var uvVec = new Aspose.ThreeD.Utilities.Vector4( uvVector.X * scaleFactor, uvVector.Y * scaleFactor, 0, 0 );
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