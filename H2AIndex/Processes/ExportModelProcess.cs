using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Assimp;
using H2AIndex.Common;
using H2AIndex.Common.Enumerations;
using H2AIndex.Models;
using H2AIndex.Services.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Saber3D.Data.Textures;
using Saber3D.Files;
using Saber3D.Files.FileTypes;
using Saber3D.Serializers.Configurations;

namespace H2AIndex.Processes
{

  public class ExportModelProcess : ProcessBase
  {

    #region Data Members


    private IS3DFile _file;
    private Scene _scene;
    private ModelExportOptionsModel _modelOptions;
    private TextureExportOptionsModel _textureOptions;

    private string _outputPath;

    #endregion

    #region Constructor

    public ExportModelProcess( IS3DFile file, Tuple<ModelExportOptionsModel, TextureExportOptionsModel> options )
    {
      _file = file;
      _modelOptions = options.Item1.DeepCopy();
      _textureOptions = options.Item2.DeepCopy();

      _textureOptions.OutputPath = _modelOptions.OutputPath;
      _textureOptions.ExportAllMips = false;
    }

    public ExportModelProcess( IS3DFile file, Scene scene, Tuple<ModelExportOptionsModel, TextureExportOptionsModel> options )
      : this( file, options )
    {
      _scene = scene;
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializing()
    {
      _outputPath = GetOutputModelPath();
    }

    protected override async Task OnExecuting()
    {
      if ( _scene is null )
        await ConvertFileToAssimpScene();

      if ( _modelOptions.ExportTextures )
        FixupTextureSlotFileExtensions();

      if ( _modelOptions.RemoveLODs || _modelOptions.RemoveVolumes )
      {
        var lodRemover = new SceneLodRemover( _scene, _modelOptions );
        _scene = lodRemover.RecreateScene();
      }

      await WriteAssimpSceneToFile();

      if ( _modelOptions.ExportTextures )
        await ExportTextures();
    }

    #endregion

    #region Private Methods

    private string GetOutputModelPath()
    {
      var extension = _modelOptions.OutputFileFormat.GetFileExtension();
      var fName = Path.ChangeExtension( _file.Name, extension );

      return Path.Combine( _modelOptions.OutputPath, fName );
    }

    private async Task ConvertFileToAssimpScene()
    {
      var process = new ConvertModelToAssimpSceneProcess( _file );
      BindStatusToSubProcess( process );

      await process.Execute();
      StatusList.Merge( process.StatusList );

      _scene = process.Result;
    }

    private async Task WriteAssimpSceneToFile()
    {
      try
      {
        Status = "Writing File";
        IsIndeterminate = true;

        var formatId = _modelOptions.OutputFileFormat.ToAssimpFormatId();
        using ( var context = new AssimpContext() )
        {
          context.ExportFile( _scene, _outputPath, formatId );
        }
      }
      catch ( Exception ex )
      {
        StatusList.AddError( _file.Name, "Failed to write the model file.", ex );
        throw;
      }
    }

    private async Task ExportTextures()
    {
      IsIndeterminate = true;
      Status = "Exporting Textures";

      var files = GatherTextures();
      var process = new BulkExportTexturesProcess( files, _textureOptions );
      BindStatusToSubProcess( process );

      await process.Execute();
      StatusList.Merge( process.StatusList );
    }

    private IEnumerable<IS3DFile> GatherTextures()
    {
      var textures = new Dictionary<string, IS3DFile>();

      // Get base textures
      foreach ( var material in _scene.Materials )
      {
        if ( material.Name == "DefaultMaterial" )
          continue;

        var textureFiles = H2AFileContext.Global
          .GetFiles( material.Name )
          .OfType<PictureFile>();

        foreach ( var file in textureFiles )
          textures.TryAdd( file.Name, file );
      }

      // Get Detail Maps and addl textures from TextureDefinition
      var textureNames = textures.Keys.ToArray();
      foreach ( var textureName in textureNames )
        GatherDetailMaps( textureName, textures );

      return textures.Values;
    }

    private void GatherDetailMaps( string parentTextureName, Dictionary<string, IS3DFile> textures )
    {
      var tdFile = H2AFileContext.Global.GetFile( Path.ChangeExtension( parentTextureName, ".td" ) );
      if ( tdFile is null )
        return;

      var texDef = new FileScriptingSerializer<TextureDefinition>().Deserialize( tdFile.GetStream() );
      foreach ( var textureName in texDef.GetTextureNames() )
      {
        var nameWithExt = Path.ChangeExtension( textureName, "pct" );
        if ( textures.ContainsKey( nameWithExt ) )
          continue;

        var textureFile = H2AFileContext.Global.GetFile( nameWithExt );
        if ( textureFile is null )
          continue;

        textures.Add( textureFile.Name, textureFile );
      }
    }

    private void FixupTextureSlotFileExtensions()
    {
      foreach ( var material in _scene.Materials )
      {
        material.TextureDiffuse = FixupTextureSlotFileExtension( material.TextureDiffuse );
        material.TextureNormal = FixupTextureSlotFileExtension( material.TextureNormal );
        material.TextureSpecular = FixupTextureSlotFileExtension( material.TextureSpecular );
      }
    }

    private TextureSlot FixupTextureSlotFileExtension( TextureSlot slot )
    {
      if ( string.IsNullOrEmpty( slot.FilePath ) )
        return slot;

      var ext = _textureOptions.OutputFileFormat.ToString().ToLower();
      var newName = Path.ChangeExtension( slot.FilePath, ext );

      slot = new TextureSlot( newName, slot.TextureType, slot.TextureIndex, slot.Mapping,
        slot.UVIndex, slot.BlendFactor, slot.Operation, slot.WrapModeU, slot.WrapModeV, slot.Flags );
      return slot;
    }

    private IEnumerable<Node> Traverse( Node node )
    {
      yield return node;
      foreach ( var child in node.Children )
      {
        yield return child;
        foreach ( var childNode in Traverse( child ) )
          yield return childNode;
      }
    }

    #endregion

  }

  internal class SceneLodRemover
  {

    #region Data Members

    private readonly IMeshIdentifierService _meshIdentifierService;

    #endregion

    #region Properties

    private Scene OldScene { get; }
    private Scene NewScene { get; }

    private Func<string, bool> Evaluate { get; }
    private Dictionary<int, int> MaterialLookup { get; }
    private Dictionary<int, int> MeshLookup { get; }

    #endregion

    #region Constructor

    public SceneLodRemover( Scene scene, ModelExportOptionsModel options )
    {
      var serviceProvider = ( ( App ) App.Current ).ServiceProvider;
      _meshIdentifierService = serviceProvider.GetRequiredService<IMeshIdentifierService>();

      OldScene = scene;
      NewScene = new Scene();
      Evaluate = CreateEvaluator( options.RemoveLODs, options.RemoveVolumes );

      MaterialLookup = new Dictionary<int, int>();
      MeshLookup = new Dictionary<int, int>();
    }

    #endregion

    #region Public Methods

    public Scene RecreateScene()
    {
      if ( Evaluate is null )
        return OldScene;

      var oldRoot = OldScene.RootNode;
      var newRoot = AddNode( oldRoot );

      NewScene.RootNode = newRoot;
      return NewScene;
    }

    #endregion

    #region Private Methods

    private Func<string, bool> CreateEvaluator( bool removeLods = false, bool removeVolumes = false )
    {
      if ( removeLods && removeVolumes )
        return _meshIdentifierService.IsLodOrVolume;
      else if ( removeLods )
        return _meshIdentifierService.IsLod;
      else if ( removeVolumes )
        return _meshIdentifierService.IsVolume;
      else
        return null;
    }

    private int AddMaterial( Material material )
    {
      NewScene.Materials.Add( material );
      return NewScene.Materials.Count - 1;
    }

    private int AddMesh( Mesh oldMesh )
    {
      var newMesh = new Mesh( oldMesh.Name, oldMesh.PrimitiveType );
      newMesh.BiTangents.AddRange( oldMesh.BiTangents );
      newMesh.Bones.AddRange( oldMesh.Bones );
      newMesh.BoundingBox = oldMesh.BoundingBox;
      newMesh.Faces.AddRange( oldMesh.Faces );
      newMesh.MeshAnimationAttachments.AddRange( oldMesh.MeshAnimationAttachments );
      newMesh.MorphMethod = oldMesh.MorphMethod;
      newMesh.Normals.AddRange( oldMesh.Normals );
      newMesh.Tangents.AddRange( oldMesh.Tangents );
      newMesh.Vertices.AddRange( oldMesh.Vertices );

      for ( var i = 0; i < oldMesh.TextureCoordinateChannels.Length; i++ )
        newMesh.TextureCoordinateChannels[ i ].AddRange( oldMesh.TextureCoordinateChannels[ i ] );

      for ( var i = 0; i < oldMesh.UVComponentCount.Length; i++ )
        newMesh.UVComponentCount[ i ] = oldMesh.UVComponentCount[ i ];

      for ( var i = 0; i < oldMesh.VertexColorChannels.Length; i++ )
        newMesh.VertexColorChannels[ i ].AddRange( oldMesh.VertexColorChannels[ i ] );

      if ( !MaterialLookup.TryGetValue( oldMesh.MaterialIndex, out var newMaterialIndex ) )
      {
        var material = OldScene.Materials[ oldMesh.MaterialIndex ];
        newMaterialIndex = AddMaterial( material );

        MaterialLookup.Add( oldMesh.MaterialIndex, newMaterialIndex );
      }
      newMesh.MaterialIndex = newMaterialIndex;

      NewScene.Meshes.Add( newMesh );
      return NewScene.Meshes.Count - 1;
    }

    private void CopyNodeMeshes( Node oldNode, Node newNode )
    {
      foreach ( var oldMeshIndex in oldNode.MeshIndices )
      {
        if ( !MeshLookup.TryGetValue( oldMeshIndex, out var newMeshIndex ) )
        {
          var mesh = OldScene.Meshes[ oldMeshIndex ];
          newMeshIndex = AddMesh( mesh );

          MeshLookup.Add( oldMeshIndex, newMeshIndex );
        }

        newNode.MeshIndices.Add( newMeshIndex );
      }
    }

    private Node AddNode( Node oldNode, Node newParentNode = null )
    {
      if ( Evaluate( oldNode.Name ) )
        return null;

      var newNode = new Node( oldNode.Name, newParentNode );
      if ( newParentNode != null )
        newParentNode.Children.Add( newNode );

      newNode.Transform = oldNode.Transform;

      CopyNodeMeshes( oldNode, newNode );

      foreach ( var child in oldNode.Children )
        AddNode( child, newNode );

      return newNode;
    }

    #endregion

  }

}
