using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using H2AIndex.Common;
using H2AIndex.Models;
using H2AIndex.Processes;
using H2AIndex.Services;
using H2AIndex.ViewModels.Abstract;
using H2AIndex.Views;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.Wpf.SharpDX;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
using Saber3D.Files;
using Saber3D.Files.FileTypes;
using TextureModel = HelixToolkit.SharpDX.Core.TextureModel;

namespace H2AIndex.ViewModels
{

  [AcceptsFileType( typeof( TplFile ) )]
  [AcceptsFileType( typeof( LgFile ) )]
  public class ModelViewModel : ViewModel, IDisposeWithView
  {

    #region Data Members

    private IS3DFile _file;

    private string _searchTerm;
    private ObservableCollection<ModelNodeModel> _nodes;
    private ICollectionView _nodeCollectionView;

    private Assimp.Scene _assimpScene;

    #endregion

    #region Properties

    public ModelViewerOptionsModel Options { get; set; }

    public Camera Camera { get; set; }
    public SceneNodeGroupModel3D Model { get; }
    public EffectsManager EffectsManager { get; }
    public Viewport3DX Viewport { get; set; }

    [OnChangedMethod( nameof( ToggleShowWireframe ) )]
    public bool ShowWireframe { get; set; }

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

      EffectsManager = new DefaultEffectsManager();
      Camera = new PerspectiveCamera() { FarPlaneDistance = 300000 };
      Model = new SceneNodeGroupModel3D();

      var transform = new System.Windows.Media.Media3D.RotateTransform3D();
      transform.Rotation = new System.Windows.Media.Media3D.AxisAngleRotation3D(
        new System.Windows.Media.Media3D.Vector3D( 0, 1, 0 ), -90 );
      Model.Transform = transform;

      _nodes = new ObservableCollection<ModelNodeModel>();
      _nodeCollectionView = InitializeNodeCollectionView( _nodes );

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

      var convertProcess = new ConvertModelToAssimpSceneProcess( _file );
      await RunProcess( convertProcess );

      _assimpScene = convertProcess.Result;

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
      collectionView.Filter = ( obj ) =>
      {
        if ( string.IsNullOrEmpty( _searchTerm ) )
          return true;

        var node = obj as ModelNodeModel;
        return node.Name.Contains( _searchTerm, StringComparison.InvariantCultureIgnoreCase );
      };

      return collectionView;
    }

    private async Task<TextureModel> GetTexture( string name )
    {
      var file = H2AFileContext.Global.GetFiles( name ).FirstOrDefault();
      if ( file is null )
        return null;

      var svc = ServiceProvider.GetService<ITextureConversionService>();
      var stream = await svc.GetDDSStream( file );

      return TextureModel.Create( stream );
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

      var matLookup = new Dictionary<string, Material>();
      foreach ( var node in scene.Root.Traverse() )
      {
        if ( node is MeshNode mn )
        {
          var matName = mn.Material.Name;
          if ( !matLookup.TryGetValue( matName, out var mat ) )
          {
            mat = new PhongMaterial
            {
              DiffuseMap = await GetTexture( $"{matName}.pct" ),
              SpecularColorMap = await GetTexture( $"{matName}_spec.pct" ),
              NormalMap = await GetTexture( $"{matName}_nm.pct" ),
              EnableTessellation = true,
              UVTransform = new UVTransform( 0, 1, -1, 0, 0 )
            };
            matLookup.Add( matName, mat );
          }

          mn.Material = mat;
        }
      }


      Model.AddNode( scene.Root );
      Camera.ZoomExtents( Viewport );
      UpdateMeshInfo();
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
        if ( node.Name.Contains( "LOD1", StringComparison.InvariantCultureIgnoreCase )
          || node.Name.Contains( "LOD2", StringComparison.InvariantCultureIgnoreCase )
          || node.Name.Contains( "LOD3", StringComparison.InvariantCultureIgnoreCase )
          || node.Name.Contains( "lod01", StringComparison.InvariantCultureIgnoreCase )
          || node.Name.Contains( "lod02", StringComparison.InvariantCultureIgnoreCase )
          || node.Name.Contains( "lod03", StringComparison.InvariantCultureIgnoreCase ) )
          node.IsVisible = false;
      }
    }

    private void HideVolumeNodes()
    {
      foreach ( var node in Traverse( _nodes ) )
      {
        if ( node.Name.Contains( "shadow", StringComparison.InvariantCultureIgnoreCase )
          || node.Name.Contains( "occl", StringComparison.InvariantCultureIgnoreCase )
          || node.Name.Contains( "oclud", StringComparison.InvariantCultureIgnoreCase )
          || node.Name.Contains( "refl", StringComparison.InvariantCultureIgnoreCase ) )
          node.IsVisible = false;
        else if ( node.Node is MeshNode mn )
          if ( mn.Material is null )
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
        if ( node.Node is MeshNode meshNode )
        {
          meshNode.RenderWireframe = show;
        }
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

    private async Task ExportModel()
    {
      var result = await ShowViewModal<ModelExportOptionsView>();
      if ( !( result is Tuple<ModelExportOptionsModel, TextureExportOptionsModel> options ) )
        return;

      var exportProcess = new ExportModelProcess( _file, _assimpScene, options );
      await RunProcess( exportProcess );
    }

    #endregion

  }

  public class ModelNodeModel : ObservableObject
  {

    private SceneNode _node;

    public SceneNode Node => _node;
    public bool IsExpanded { get; set; }

    public string Name => _node.Name;

    public ICollection<ModelNodeModel> Items { get; }

    [OnChangedMethod( nameof( OnNodeVisibilityChanged ) )]
    public bool IsVisible { get; set; }

    public ModelNodeModel( SceneNode node )
    {
      _node = node;
      node.Tag = this;
      Items = new ObservableCollection<ModelNodeModel>();

      IsExpanded = true;
      IsVisible = true;
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

  }

}