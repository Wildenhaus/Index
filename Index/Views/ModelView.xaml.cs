using System.IO;
using System.Text;
using System.Threading.Tasks;
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

    #endregion

    #region Constructor

    public ModelView( IS3DFile file )
    {
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
        var geometryGraph = DeserializeModel( _file );

        prog.Header = "Converting Model";
        var stream = _file.GetStream();
        var reader = new BinaryReader( stream, Encoding.UTF8, true );
        var assimpScene = SceneExporter.CreateScene( _file.Name, geometryGraph, reader, prog );

        prog.Header = "Preparing Viewer";
        var wpfModel = WpfModelHelper.PrepareModelForViewer( assimpScene, prog );
        wpfModel.Freeze();

        ViewModel.Model = wpfModel;

        ModelViewer.Dispatcher.Invoke( () => ModelViewer.ZoomToBounds( wpfModel ) );
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

    #endregion


  }

}
