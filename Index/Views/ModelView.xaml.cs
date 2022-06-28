using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Index.Common;
using Index.Tools;
using Index.ViewModels;
using Saber3D.Data;
using Saber3D.Files;
using Saber3D.Serializers;

namespace Index.Views
{

  public partial class ModelView : View
  {

    #region Data Members

    private IS3DFile _file;

    #endregion

    #region Properties

    public ModelViewViewModel ViewModel { get; }

    public ICommand ExportCommand { get; }

    #endregion

    #region Constructor

    public ModelView( IS3DFile file )
    {
      ExportCommand = new DelegateCommand( ExportModel );
      InitializeComponent();
      _file = file;

      ViewModel = new ModelViewViewModel();
      DataContext = ViewModel;
    }

    #endregion

    #region Overrides

    protected override Task OnInitializing()
    {
      ViewModel.Name = _file.Name;

      return AppManager.PerformWork( Host, prog =>
      {
        prog.IsIndeterminate = true;

        prog.Header = "Deserializing Model";
        S3DGeometryGraph geometryGraph;
        try
        {
          geometryGraph = DeserializeModel( _file );
        }
        catch ( Exception ex )
        {
          AppManager.ShowMessageModal( Host,
            title: "Error",
            message: $"Encountered an error while deserializing the model:\n{ex.Message}" );
          return;
        }

        prog.Header = "Converting Model";
        try
        {
          var stream = _file.GetStream();
          var reader = new BinaryReader( stream, Encoding.UTF8, true );
          ViewModel.AssimpScene = SceneExporter.CreateScene( _file.Name, geometryGraph, reader, prog );
        }
        catch ( Exception ex )
        {
          AppManager.ShowMessageModal( Host,
            title: "Error",
            message: $"Encountered an error while converting the model:\n{ex.Message}" );
          return;
        }

        prog.Header = "Preparing Viewer";
        try
        {
          var wpfModel = WpfModelHelper.PrepareModelForViewer( ViewModel.AssimpScene, prog );

          ViewModel.Model = wpfModel;
          ModelViewer.Dispatcher.Invoke( () => ModelViewer.ZoomToBounds( wpfModel ) );
        }
        catch ( Exception ex )
        {
          AppManager.ShowMessageModal( Host,
            title: "Error",
            message: $"Encountered an error while preparing the model viewer:\n{ex.Message}" );
          return;
        }

      } );
    }

    protected override void OnDisposing()
    {
      ModelViewer?.Dispose();
      ViewModel.Dispose();

      AppManager.ForceGarbageCollection();
    }

    #endregion

    #region Private Methods

    private static S3DGeometryGraph DeserializeModel( IS3DFile file )
    {
      var stream = file.GetStream();
      var reader = new BinaryReader( stream, Encoding.UTF8, true );

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

    private async void ExportModel( object param )
    {
    }

    #endregion


  }

}
