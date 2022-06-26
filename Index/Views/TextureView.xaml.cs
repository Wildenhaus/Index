using System.IO;
using System.Text;
using System.Threading.Tasks;
using Index.Models;
using Index.Tools;
using Index.ViewModels;
using Saber3D.Data.Textures;
using Saber3D.Files;
using Saber3D.Serializers;

namespace Index.Views
{

  public partial class TextureView : View
  {

    #region Data Members

    private IS3DFile _file;

    #endregion

    #region Properties

    public TextureViewViewModel ViewModel { get; set; }

    #endregion

    #region Constructor

    public TextureView( IS3DFile file )
    {
      InitializeComponent();
      _file = file;

      ViewModel = new TextureViewViewModel();
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

        prog.Header = "Deserializing Texture";
        var pict = DeserializePicture( _file );
        ViewModel.ApplyMetadata( pict );

        prog.Header = "Generating Texture Previews";
        foreach ( var image in TextureConverter.ConvertToBitmaps( pict ) )
          Dispatcher.Invoke( () =>
          {
            var imageModel = new TextureImageModel
            {
              Index = ViewModel.Images.Count,
              ImageData = image
            };

            ViewModel.Images.Add( imageModel );
          } );

        ViewModel.SelectedTexture = ViewModel.Images[ 0 ];
      } );
    }

    protected override void OnDisposing()
    {
      ViewModel.Dispose();
      AppManager.ForceGarbageCollection();
    }

    #endregion

    #region Private Methods

    private static S3DPicture DeserializePicture( IS3DFile file )
    {
      var stream = file.GetStream();
      var reader = new BinaryReader( stream, Encoding.UTF8, true );

      return new S3DPictureSerializer().Deserialize( reader );
    }

    #endregion

  }

}
