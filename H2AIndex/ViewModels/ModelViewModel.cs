using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Assimp;
using H2AIndex.Common;
using H2AIndex.Services.Abstract;
using H2AIndex.Tools;
using H2AIndex.ViewModels.Abstract;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.Wpf.SharpDX;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
using Saber3D.Data;
using Saber3D.Files;
using Saber3D.Files.FileTypes;
using Saber3D.Serializers;

namespace H2AIndex.ViewModels
{

  [AcceptsFileType( typeof( TplFile ) )]
  [AcceptsFileType( typeof( LgFile ) )]
  public class ModelViewModel : ViewModel, IDisposeWithView
  {

    private IS3DFile _file;

    #region Properties

    public HelixToolkit.Wpf.SharpDX.Camera Camera { get; set; }
    public SceneNodeGroupModel3D Model { get; }
    public EffectsManager EffectsManager { get; }

    #endregion

    public ModelViewModel( IServiceProvider serviceProvider, IS3DFile file )
      : base( serviceProvider )
    {
      _file = file;

      EffectsManager = new DefaultEffectsManager();
      Camera = new PerspectiveCamera()
      {
        FarPlaneDistance = 30000
      };
      Model = new SceneNodeGroupModel3D();
    }

    protected override async Task OnInitializing()
    {
      using ( var prog = ShowProgress() )
      {
        prog.IsIndeterminate = true;

        prog.Status = "Deserializing Model";
        S3DGeometryGraph geometryGraph;
        try
        {
          geometryGraph = DeserializeModel( _file );
        }
        catch ( Exception ex )
        {
          ShowExceptionModal( ex );
          return;
        }

        Scene scene;
        prog.Status = "Converting Model";
        try
        {
          var stream = _file.GetStream();
          var reader = new BinaryReader( stream );
          scene = SceneExporter.CreateScene( _file.Name, geometryGraph, reader, prog );
        }
        catch ( Exception ex )
        {
          ShowExceptionModal( ex );
          return;
        }

        prog.Status = "Preparing Viewer";
        prog.IsIndeterminate = true;

        await Task.Run( () =>
        {
          var importer = new Importer();
          var result = importer.ToHelixToolkitScene( scene, out var hxScene );
          return hxScene;
        } ).ContinueWith( async ( res ) =>
        {
          var scene = res.Result;

          foreach ( var node in scene.Root.Traverse() )
          {
            node.Tag = new ModelNodeModel( node );
            if ( node is MeshNode mn )
            {
              var matName = mn.Material.Name;
              var mat = new PBRMaterial()
              {
                AlbedoMap = await GetTexture( $"{matName}.pct" ),
                NormalMap = await GetTexture( $"{matName}_nm.pct" ),
                EnableTessellation = true,
                UVTransform = new HelixToolkit.SharpDX.Core.UVTransform( 0, 1, -1, 0, 0 )
              };
              mn.Material = mat;
            }
          }

          Model.AddNode( scene.Root );
        }, TaskScheduler.FromCurrentSynchronizationContext() );
      }
    }

    protected override void OnDisposing()
    {
      Model?.Dispose();
      EffectsManager?.DisposeAllResources();
      GCHelper.ForceCollect();
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

    private static S3DGeometryGraph DeserializeModel( IS3DFile file )
    {
      var stream = file.GetStream();
      var reader = new BinaryReader( stream );

      if ( file.Extension == ".tpl" )
      {
        var tpl = new S3DTemplateSerializer().Deserialize( reader );
        return tpl.GeometryGraph;
      }
      else if ( file.Extension == ".lg" )
      {
        var lg = new S3DSceneSerializer().Deserialize( reader );
        return lg.GeometryGraph;
      }
      else
        return null;
    }

  }

  public class ModelNodeModel : ObservableObject
  {

    private SceneNode _node;

    public bool IsExpanded { get; set; }

    public string Name => _node.Name;

    [OnChangedMethod( nameof( OnNodeVisibilityChanged ) )]
    public bool IsVisible { get; set; }

    public ModelNodeModel( SceneNode node )
    {
      _node = node;
      node.Tag = this;

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