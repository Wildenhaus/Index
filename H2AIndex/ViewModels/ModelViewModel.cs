using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using H2AIndex.Common;
using H2AIndex.Models;
using H2AIndex.Processes;
using H2AIndex.Services;
using H2AIndex.Services.Abstract;
using H2AIndex.ViewModels.Abstract;
using H2AIndex.Views;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.Wpf.SharpDX;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
using Saber3D.Data;
using Saber3D.Files;
using Saber3D.Files.FileTypes;
using MaterialCore = HelixToolkit.SharpDX.Core.Model.MaterialCore;
using PhongMaterialCore = HelixToolkit.SharpDX.Core.Model.PhongMaterialCore;
using TextureModel = HelixToolkit.SharpDX.Core.TextureModel;

namespace H2AIndex.ViewModels
{

  [AcceptsFileType( typeof( TplFile ) )]
  [AcceptsFileType( typeof( LgFile ) )]
  public class ModelViewModel : ViewModel, IDisposeWithView
  {

    #region Data Members

    private readonly IH2AFileContext _fileContext;
    private readonly IMeshIdentifierService _meshIdentifierService;

    private IS3DFile _file;

    private string _searchTerm;
    private ObservableCollection<ModelNodeModel> _nodes;
    private ICollectionView _nodeCollectionView;

    private Assimp.Scene _assimpScene;
    private S3DGeometryGraph _geometryGraph;
    private Dictionary<string, TextureModel> _loadedTextures;

    #endregion

    #region Properties

    public ModelViewerOptionsModel Options { get; set; }

    public Camera Camera { get; set; }
    public SceneNodeGroupModel3D Model { get; }
    public EffectsManager EffectsManager { get; }
    public Viewport3DX Viewport { get; set; }

    public double MinMoveSpeed { get; set; }
    public double MoveSpeed { get; set; }
    public double MaxMoveSpeed { get; set; }

    [OnChangedMethod( nameof( ToggleShowWireframe ) )]
    public bool ShowWireframe { get; set; }
    [OnChangedMethod( nameof( ToggleShowTextures ) )]
    public bool ShowTextures { get; set; }
    public bool UseFlycam { get; set; }

    public ICollectionView Nodes => _nodeCollectionView;
    public int MeshCount { get; set; }
    public int VertexCount { get; set; }
    public int FaceCount { get; set; }

    public ICommand SearchTermChangedCommand { get; }

    public ICommand ShowAllCommand { get; }
    public ICommand HideAllCommand { get; }
    public ICommand HideLODsCommand { get; }
    public ICommand HideVolumesCommand { get; }
    public ICommand ExpandAllCommand { get; }
    public ICommand CollapseAllCommand { get; }

    public ICommand ExportModelCommand { get; }

    #endregion

    #region Constructor

    public ModelViewModel( IServiceProvider serviceProvider, IS3DFile file )
      : base( serviceProvider )
    {
      _file = file;
      _loadedTextures = new Dictionary<string, TextureModel>();

      _fileContext = ServiceProvider.GetRequiredService<IH2AFileContext>();
      _meshIdentifierService = ServiceProvider.GetRequiredService<IMeshIdentifierService>();

      EffectsManager = new DefaultEffectsManager();
      Camera = new PerspectiveCamera() { FarPlaneDistance = 300000 };
      Model = new SceneNodeGroupModel3D();
      ApplyTransformsToModel();

      _nodes = new ObservableCollection<ModelNodeModel>();
      _nodeCollectionView = InitializeNodeCollectionView( _nodes );

      ShowTextures = true;

      ShowAllCommand = new Command( ShowAllNodes );
      HideAllCommand = new Command( HideAllNodes );
      HideLODsCommand = new Command( HideLODNodes );
      HideVolumesCommand = new Command( HideVolumeNodes );
      ExpandAllCommand = new Command( ExpandAllNodes );
      CollapseAllCommand = new Command( CollapseAllNodes );
      SearchTermChangedCommand = new Command<string>( SearchTermChanged );

      ExportModelCommand = new AsyncCommand( ExportModel );
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializing()
    {
      Options = GetPreferences().ModelViewerOptions;
      UseFlycam = Options.DefaultToFlycam;

      var convertProcess = new ConvertModelToAssimpSceneProcess( _file );
      await RunProcess( convertProcess );

      _assimpScene = convertProcess.Result;
      _geometryGraph = convertProcess.GeometryGraph;

      using ( var prog = ShowProgress() )
      {
        prog.Status = "Preparing Viewer";
        prog.IsIndeterminate = true;

        await PrepareModelViewer( convertProcess.Result );
      };
    }

    protected override void OnDisposing()
    {
      Model?.Dispose();
      EffectsManager?.DisposeAllResources();
      GCHelper.ForceCollect();
    }

    #endregion

    #region Private Methods

    private ICollectionView InitializeNodeCollectionView( ObservableCollection<ModelNodeModel> files )
    {
      var collectionView = CollectionViewSource.GetDefaultView( _nodes );
      collectionView.SortDescriptions.Add( new SortDescription( nameof( ModelNodeModel.Name ), ListSortDirection.Ascending ) );
      collectionView.Filter = ( obj ) =>
      {
        if ( string.IsNullOrEmpty( _searchTerm ) )
          return true;

        var node = obj as ModelNodeModel;
        return node.Name.Contains( _searchTerm, StringComparison.InvariantCultureIgnoreCase );
      };

      return collectionView;
    }

    private async Task<TextureModel> LoadTexture( string name )
    {
      if ( string.IsNullOrWhiteSpace( name ) )
        return null;

      name = Path.ChangeExtension( name, ".pct" );
      if ( _loadedTextures.TryGetValue( name, out var texture ) )
        return texture;

      var file = _fileContext.GetFile<PictureFile>( name );
      if ( file is null )
        return null;

      var svc = ServiceProvider.GetService<ITextureConversionService>();
      var stream = await svc.GetJpgStream( file, Options.ModelTexturePreviewQuality );

      texture = TextureModel.Create( stream );
      _loadedTextures.Add( name, texture );
      return texture;
    }

    private async void ApplyTexturesToNode( MeshNode meshNode )
    {
      var nodeMaterial = meshNode.Material as PhongMaterialCore;
      if ( nodeMaterial is null )
        return;

      var baseTexName = nodeMaterial.DiffuseMapFilePath;
      if ( string.IsNullOrWhiteSpace( baseTexName ) )
        return;

      nodeMaterial.DiffuseMap = await LoadTexture( $"{baseTexName}.pct" );
      nodeMaterial.SpecularColorMap = await LoadTexture( $"{baseTexName}_spec.pct" );
      //nodeMaterial.NormalMap = await LoadTexture( $"{baseTexName}_nm.pct" );
      nodeMaterial.EnableTessellation = true;
      nodeMaterial.UVTransform = new UVTransform( 0, 1, -1, 0, 0 );
    }

    private void ApplyTransformsToModel()
    {
      var transformGroup = new System.Windows.Media.Media3D.Transform3DGroup();

      var rotTransform = new System.Windows.Media.Media3D.RotateTransform3D();
      rotTransform.Rotation = new System.Windows.Media.Media3D.AxisAngleRotation3D(
        new System.Windows.Media.Media3D.Vector3D( 0, 1, 0 ), -90 );
      transformGroup.Children.Add( rotTransform );

      var scaleTransform = new System.Windows.Media.Media3D.ScaleTransform3D( 100, 100, 100 );
      transformGroup.Children.Add( scaleTransform );

      Model.Transform = transformGroup;
    }

    private async Task PrepareModelViewer( Assimp.Scene assimpScene )
    {
      var importer = new Importer();
      importer.ToHelixToolkitScene( assimpScene, out var scene );

      void AddNodeModels( SceneNode node )
      {
        if ( node is MeshNode )
        {
          var nodeModel = new ModelNodeModel( node );
          _nodes.Add( nodeModel );
        }

        foreach ( var childNode in node.Items )
          AddNodeModels( childNode );
      }

      AddNodeModels( scene.Root );

      var materialLookup = new Dictionary<string, Material>();
      foreach ( var node in scene.Root.Traverse() )
      {
        if ( node is MeshNode meshNode )
          ApplyTexturesToNode( meshNode );
      }

      App.Current.Dispatcher.Invoke( () =>
      {
        Model.AddNode( scene.Root );
        CalculateMoveSpeed( scene );
        Camera.ZoomExtents( Viewport );
        UpdateMeshInfo();
      } );
    }

    private void ShowAllNodes()
    {
      foreach ( var node in Traverse( _nodes ) )
        node.IsVisible = true;
    }

    private void HideAllNodes()
    {
      foreach ( var node in Traverse( _nodes ) )
        node.IsVisible = false;
    }

    private void HideLODNodes()
    {
      foreach ( var node in Traverse( _nodes ) )
      {
        if ( _meshIdentifierService.IsLod( node.Name ) )
          node.IsVisible = false;
      }
    }

    private void HideVolumeNodes()
    {
      foreach ( var node in Traverse( _nodes ) )
      {
        if ( _meshIdentifierService.IsVolume( node.Name ) )
          node.IsVisible = false;
      }
    }

    private void ExpandAllNodes()
    {
      foreach ( var node in Traverse( _nodes ) )
        node.IsExpanded = true;
    }

    private void CollapseAllNodes()
    {
      foreach ( var node in Traverse( _nodes ) )
        node.IsExpanded = false;
    }

    private void SearchTermChanged( string searchTerm )
    {
      _searchTerm = searchTerm;
      App.Current.Dispatcher.Invoke( _nodeCollectionView.Refresh );
    }

    private void ToggleShowWireframe()
    {
      var show = ShowWireframe;
      foreach ( var node in Traverse( _nodes ) )
        node.ShowWireframe = show;
    }

    private void ToggleShowTextures()
    {
      var show = ShowTextures;
      foreach ( var node in Traverse( _nodes ) )
        node.ShowTexture = show;
    }

    private IEnumerable<ModelNodeModel> Traverse( IEnumerable<ModelNodeModel> rootElems )
    {
      foreach ( var elem in rootElems )
      {
        yield return elem;
        foreach ( var child in Traverse( elem.Items ) )
          yield return child;
      }
    }

    private void UpdateMeshInfo()
    {
      var meshCount = 0;
      var vertCount = 0;
      var faceCount = 0;

      foreach ( var node in Traverse( _nodes ).Select( x => x.Node ).OfType<MeshNode>() )
      {
        if ( !node.Visible )
          continue;

        meshCount++;
        vertCount += node.Geometry.Positions.Count;
        faceCount += node.Geometry.Indices.Count / 3;
      }

      MeshCount = meshCount;
      VertexCount = vertCount;
      FaceCount = faceCount;
    }

    private void CalculateMoveSpeed( HelixToolkitScene scene )
    {
      double maxW = 0, maxH = 0, maxD = 0;
      foreach ( var node in scene.Root.Traverse() )
      {
        var mn = node as MeshNode;
        if ( mn is null )
          continue;

        maxW = Math.Max( maxW, mn.Bounds.Width );
        maxH = Math.Max( maxH, mn.Bounds.Height );
        maxD = Math.Max( maxD, mn.Bounds.Depth );
      }

      const double BASELINE_MAX_DIM = 2.31;
      const double BASELINE_MIN_SPEED = 0.0001;
      const double BASELINE_DEFAULT_SPEED = 0.001;
      const double BASELINE_MAX_SPEED = 1;

      var maxDim = Math.Max( maxW, Math.Max( maxH, maxD ) );
      var coef = maxDim / BASELINE_MAX_DIM;

      MinMoveSpeed = BASELINE_MIN_SPEED * coef;
      MoveSpeed = BASELINE_DEFAULT_SPEED * coef;
      MaxMoveSpeed = BASELINE_MAX_SPEED * coef;
    }

    private async Task ExportModel()
    {
      var result = await ShowViewModal<ModelExportOptionsView>();
      if ( !( result is Tuple<ModelExportOptionsModel, TextureExportOptionsModel> options ) )
        return;

      var modelOptions = options.Item1;
      var textureOptions = options.Item2;

      var exportProcess = new ExportModelProcess( _file, _assimpScene, _geometryGraph, modelOptions, textureOptions );
      await RunProcess( exportProcess );
    }

    #endregion

  }

  public class ModelNodeModel : ObservableObject
  {

    #region Data Members

    private static readonly MaterialCore DEFAULT_MATERIAL
      = new DiffuseMaterial();

    private SceneNode _node;
    private MaterialCore _material;

    #endregion

    #region Properties

    public SceneNode Node => _node;
    public string Name => _node.Name;
    public ICollection<ModelNodeModel> Items { get; }

    public bool IsExpanded { get; set; }
    [OnChangedMethod( nameof( OnNodeVisibilityChanged ) )]
    public bool IsVisible { get; set; }
    [OnChangedMethod( nameof( OnShowTextureChanged ) )]
    public bool ShowTexture { get; set; }
    [OnChangedMethod( nameof( OnShowWireframeChanged ) )]
    public bool ShowWireframe { get; set; }

    #endregion

    #region Constructor

    public ModelNodeModel( SceneNode node )
    {
      _node = node;
      if ( node is MeshNode meshNode )
        _material = meshNode.Material;

      node.Tag = this;
      Items = new ObservableCollection<ModelNodeModel>();

      IsExpanded = true;
      IsVisible = true;

      ShowTexture = true;
      ShowWireframe = false;
    }

    #endregion

    #region Event Handlers

    private void OnShowTextureChanged()
    {
      if ( _node is MeshNode meshNode )
      {
        if ( ShowTexture )
          meshNode.Material = _material;
        else
          meshNode.Material = DEFAULT_MATERIAL;
      }
    }

    private void OnShowWireframeChanged()
    {
      if ( _node is MeshNode meshNode )
        meshNode.RenderWireframe = ShowWireframe;
    }

    private void OnNodeVisibilityChanged()
    {
      _node.Visible = IsVisible;
      foreach ( var childNode in _node.Traverse() )
      {
        var nodeModel = ( childNode.Tag as ModelNodeModel );
        if ( nodeModel is null )
          continue;

        nodeModel.IsVisible = IsVisible;
      }
    }

    #endregion

  }

}