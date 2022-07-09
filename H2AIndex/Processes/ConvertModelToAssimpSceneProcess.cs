using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Assimp;
using Saber3D.Data;
using Saber3D.Data.Geometry;
using Saber3D.Files;
using Saber3D.Files.FileTypes;
using Saber3D.Serializers;
using Saber3D.Serializers.Geometry;

namespace H2AIndex.Processes
{

  public class ConvertModelToAssimpSceneProcess : ProcessBase<Scene>
  {

    #region Data Members

    private readonly IS3DFile _file;
    private BinaryReader _reader;

    private SceneContext _context;

    #endregion

    #region Properties

    public override Scene Result => _context.Scene;

    #endregion

    #region Constructor

    public ConvertModelToAssimpSceneProcess( IS3DFile file )
    {
      _file = file;
    }

    #endregion

    #region Overrides

    protected override async Task OnExecuting()
    {
      _context = await DeserializeFile();

      await BuildSkinCompounds();
      await ConvertObjects();
    }

    #endregion

    #region Private Methods

    private async Task<SceneContext> DeserializeFile()
    {
      Status = "Deserializing File";
      IsIndeterminate = true;

      var stream = _file.GetStream();
      _reader = new BinaryReader( stream );

      if ( _file is TplFile )
      {
        var tpl = new S3DTemplateSerializer().Deserialize( _reader );
        var context = new SceneContext( _file.Name, tpl.GeometryGraph, _reader );
        context.AddLodDefinitions( tpl.LodDefinitions );

        return context;
      }
      else if ( _file is LgFile )
      {
        var lg = new S3DSceneSerializer().Deserialize( _reader );
        var context = new SceneContext( _file.Name, lg.GeometryGraph, _reader );

        return context;
      }
      else
        throw new InvalidDataException( "File must be TPL or LG." );
    }

    private async Task BuildSkinCompounds()
    {
      var skinCompoundIds = _context.GeometryGraph.SubMeshes
        .Select( x => x.BufferInfo.SkinCompoundId )
        .Where( x => x >= 0 )
        .Distinct();

      foreach ( var skinCompoundId in skinCompoundIds )
      {
        var skinCompoundObject = _context.GeometryGraph.Objects[ skinCompoundId ];
        var builder = new MeshBuilder( _context, skinCompoundObject, skinCompoundObject.SubMeshes.First() );
        builder.Build();

        _context.SkinCompounds[ skinCompoundId ] = builder;
      }
    }

    private async Task ConvertObjects()
    {
      Status = "Converting Objects";
      UnitName = "Objects Converted";
      CompletedUnits = 0;
      TotalUnits = _context.GeometryGraph.Objects.Count;
      IsIndeterminate = false;

      AddObject( _context.GeometryGraph.RootObject );
    }

    private void AddObject( S3DObject obj, Node parentNode = null )
    {
      if ( parentNode is null )
        parentNode = _context.Scene.RootNode;

      if ( _context.SkinCompounds.ContainsKey( obj.Id ) )
        return;

      var objectNode = new Node( obj.GetName(), parentNode );
      parentNode.Children.Add( objectNode );
      _context.Nodes.Add( obj.Id, objectNode );

      objectNode.Transform = obj.MatrixModel.ToAssimp();

      if ( obj.SubMeshes.Any() )
      {
        AddMeshData( obj, objectNode );
        CompletedUnits++;
      }
      else
      {
        CompletedUnits++;
        foreach ( var childObject in obj.EnumerateChildren() )
          AddObject( childObject, objectNode );
      }
    }

    private void AddMeshData( S3DObject obj, Node objectNode )
    {
      foreach ( var submesh in obj.SubMeshes )
      {
        var builder = new MeshBuilder( _context, obj, submesh );
        var mesh = builder.Build();

        _context.Scene.Meshes.Add( mesh );
        objectNode.MeshIndices.Add( _context.Scene.Meshes.Count - 1 );
      }
    }

    #endregion

  }


  internal class SceneContext
  {

    public Scene Scene { get; }
    public BinaryReader Reader { get; }
    public S3DGeometryGraph GeometryGraph { get; }

    public Dictionary<short, Bone> Bones { get; }
    public Dictionary<short, Node> Nodes { get; }
    public Dictionary<string, int> MaterialIndices { get; }
    public Dictionary<short, MeshBuilder> SkinCompounds { get; }
    public Dictionary<short, short> LodIndices { get; }

    public SceneContext( string name, S3DGeometryGraph graph, BinaryReader reader )
    {
      Scene = new Scene();
      Scene.RootNode = new Node( name );

      Reader = reader;
      GeometryGraph = graph;

      Bones = new Dictionary<short, Bone>();
      Nodes = new Dictionary<short, Node>();
      MaterialIndices = new Dictionary<string, int>();
      SkinCompounds = new Dictionary<short, MeshBuilder>();
      LodIndices = new Dictionary<short, short>();

      Scene.Materials.Add( new Material() { Name = "DefaultMaterial" } );
    }

    public void AddLodDefinitions( IList<S3DLodDefinition> lodDefinitions )
    {
      if ( lodDefinitions is null )
        return;

      foreach ( var lodDefinition in lodDefinitions )
        LodIndices.Add( lodDefinition.ObjectId, lodDefinition.Index );
    }

  }

  internal class MeshBuilder
  {

    private readonly SceneContext _context;
    private readonly S3DObject _object;
    private readonly S3DGeometrySubMesh _submesh;

    public Mesh Mesh { get; }
    public Dictionary<int, int> VertexLookup { get; }

    private BinaryReader Reader => _context.Reader;
    private S3DGeometryGraph Graph => _context.GeometryGraph;
    public Dictionary<short, Bone> Bones { get; }

    public MeshBuilder( SceneContext context, S3DObject obj, S3DGeometrySubMesh submesh )
    {
      _context = context;
      _object = obj;
      _submesh = submesh;

      var meshName = _object.GetName();
      if ( _context.LodIndices.TryGetValue( _object.Id, out var lodIndex ) && lodIndex > 0 )
        if ( !meshName.Contains( "LOD", System.StringComparison.InvariantCultureIgnoreCase ) )
          meshName = $"{meshName}.LOD{lodIndex}";

      Mesh = new Mesh( meshName, PrimitiveType.Triangle );

      Bones = new Dictionary<short, Bone>();
      VertexLookup = new Dictionary<int, int>();
    }

    public Mesh Build()
    {
      var meshData = Graph.Meshes[ ( int ) _submesh.MeshId ];
      foreach ( var meshBuffer in meshData.Buffers )
      {
        var buffer = Graph.Buffers[ ( int ) meshBuffer.BufferId ];

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
          case S3DGeometryElementType.BoneId:
            AddSkinCompoundBoneIds( buffer, meshBuffer );
            break;
        }
      }

      ApplySkinCompoundData();
      AddMaterial();

      return Mesh;
    }

    private void AddFaces( S3DGeometryBuffer buffer, S3DGeometryMeshBuffer meshBuffer )
    {
      var offset = _submesh.BufferInfo.FaceOffset;
      var startIndex = offset + meshBuffer.SubBufferOffset / buffer.ElementSize;
      var endIndex = startIndex + _submesh.BufferInfo.FaceCount;

      var serializer = new S3DFaceSerializer( buffer );
      foreach ( var face in serializer.DeserializeRange( Reader, ( int ) startIndex, ( int ) endIndex ) )
      {
        var assimpFace = new Face();
        assimpFace.Indices.Add( VertexLookup[ face[ 0 ] ] );
        assimpFace.Indices.Add( VertexLookup[ face[ 1 ] ] );
        assimpFace.Indices.Add( VertexLookup[ face[ 2 ] ] );

        Mesh.Faces.Add( assimpFace );
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
      foreach ( var vertex in serializer.DeserializeRange( Reader, ( int ) startIndex, ( int ) endIndex ) )
      {
        Mesh.Vertices.Add( vertex.Position.ToAssimp3D() * scale + pos );

        if ( vertex.Normal.HasValue )
          Mesh.Normals.Add( vertex.Normal.Value.ToAssimp3D() );

        if ( vertex is S3DVertexSkinned skinnedVertex )
          AddVertexSkinningData( skinnedVertex );
        else
          AddBoneWeight( _object.ParentId, 1 );

        VertexLookup.Add( offset++, VertexLookup.Count );
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

    private void AddBoneWeight( short boneObjectId, float weight, int vertIndex = -1 )
    {
      if ( boneObjectId == -1 )
        return;

      if ( vertIndex == -1 )
        vertIndex = Mesh.VertexCount - 1;

      if ( !Bones.TryGetValue( boneObjectId, out var bone ) )
      {
        var boneObject = Graph.Objects[ boneObjectId ];

        System.Numerics.Matrix4x4.Invert( boneObject.MatrixLT, out var invMatrix );

        bone = new Bone
        {
          Name = $"{boneObject.GetName()}.{boneObject.Id}",
          OffsetMatrix = invMatrix.ToAssimp()
        };

        Mesh.Bones.Add( bone );
        Bones.Add( boneObjectId, bone );
      }

      bone.VertexWeights.Add( new VertexWeight( vertIndex, weight ) );
    }

    private void AddInterleavedData( S3DGeometryBuffer buffer, S3DGeometryMeshBuffer meshBuffer )
    {
      var offset = _submesh.BufferInfo.VertexOffset;
      var startIndex = offset + ( meshBuffer.SubBufferOffset / buffer.ElementSize );
      var endIndex = startIndex + _submesh.BufferInfo.VertexCount;

      var serializer = new S3DInterleavedDataSerializer( buffer );
      foreach ( var data in serializer.DeserializeRange( Reader, ( int ) startIndex, ( int ) endIndex ) )
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
      Mesh.Tangents.Add( tangentVector.ToAssimp3D() );
    }

    private void AddVertexUV( byte uvChannel, Vector4 uvVector )
    {
      if ( !_submesh.UvScaling.TryGetValue( uvChannel, out var scaleFactor ) )
        scaleFactor = 1;

      var scaleVector = new Vector3D( scaleFactor );
      var scaledUvVector = uvVector.ToAssimp3D() * scaleVector;

      Mesh.TextureCoordinateChannels[ uvChannel ].Add( scaledUvVector );

      /* This is a bit confusing, but this property denotes the size of the UV element.
       * E.g. setting it to 2 means there is a U and a V.
       * I don't know how 4D UVs work, but if we ever add support for them, we'd need to
       * adjust this accordingly.
       */
      Mesh.UVComponentCount[ uvChannel ] = 2;
    }

    private void AddSkinCompoundBoneIds( S3DGeometryBuffer buffer, S3DGeometryMeshBuffer meshBuffer )
    {
      var offset = _submesh.BufferInfo.VertexOffset;
      var startIndex = offset + meshBuffer.SubBufferOffset / buffer.ElementSize;
      var endIndex = startIndex + _submesh.BufferInfo.VertexCount;

      var boneIds = _submesh.BoneIds;

      Reader.BaseStream.Position = buffer.StartOffset + meshBuffer.SubBufferOffset + offset * buffer.ElementSize;
      for ( var i = startIndex; i < endIndex; i++ )
      {
        var boneIndex = boneIds[ Reader.ReadInt32() ];
        var vertIndex = VertexLookup[ offset++ ];
        AddBoneWeight( boneIndex, 1, vertIndex );
      }
    }

    private void ApplySkinCompoundData()
    {
      var skinCompoundId = _submesh.BufferInfo.SkinCompoundId;
      if ( skinCompoundId == -1 )
        return;

      var skinCompound = _context.SkinCompounds[ skinCompoundId ];
      var offset = _submesh.BufferInfo.VertexOffset;
      var vertCount = _submesh.BufferInfo.VertexCount;

      var skinCompoundVertOffset = skinCompound.VertexLookup.Min( x => x.Key );

      foreach ( var bonePair in skinCompound.Bones )
      {
        var boneId = bonePair.Key;
        var bone = bonePair.Value;

        foreach ( var weight in bone.VertexWeights )
        {
          var trueVertOffset = weight.VertexID + skinCompoundVertOffset;
          if ( !VertexLookup.TryGetValue( trueVertOffset, out var translatedVertOffset ) )
            continue;

          var skinVertex = skinCompound.Mesh.Vertices[ weight.VertexID ];
          var targetVertex = Mesh.Vertices[ translatedVertOffset ];
          Debug.Assert( skinVertex.X == targetVertex.X );
          Debug.Assert( skinVertex.Y == targetVertex.Y );
          Debug.Assert( skinVertex.Z == targetVertex.Z );

          AddBoneWeight( boneId, 1, translatedVertOffset );

          var skinNorm = skinCompound.Mesh.Normals[ weight.VertexID ];
          Mesh.Normals[ translatedVertOffset ] = skinNorm;
        }
      }
    }

    private void AddMaterial()
    {
      var submeshMaterial = _submesh.Material;
      if ( submeshMaterial is null )
        return;

      var materialName = submeshMaterial.ShadingMaterialTexture;
      if ( _context.MaterialIndices.TryGetValue( materialName, out var materialIndex ) )
      {
        Mesh.MaterialIndex = materialIndex;
        return;
      }

      var material = new Material { Name = materialName };
      material.TextureDiffuse = new TextureSlot { FilePath = $"{materialName}" };
      material.TextureNormal = new TextureSlot { FilePath = $"{materialName}nm" };
      material.TextureSpecular = new TextureSlot { FilePath = $"{materialName}_spec" };

      materialIndex = _context.Scene.Materials.Count;

      _context.Scene.Materials.Add( material );
      _context.MaterialIndices.Add( materialName, materialIndex );
      Mesh.MaterialIndex = materialIndex;
    }

  }

}
