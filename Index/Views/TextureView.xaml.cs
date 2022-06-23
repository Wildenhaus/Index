using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Index.Controls;
using Index.Models;
using Index.Tools;
using PropertyChanged;
using Saber3D.Data.Textures;
using Saber3D.Files;
using Saber3D.Serializers;

namespace Index.Views
{

  public partial class TextureView : UserControl, IInitializableView
  {

    #region Data Members

    private IS3DFile _file;

    #endregion

    #region Properties

    public ContentHost Host { get; private set; }

    public TextureViewViewModel ViewModel { get; }

    #endregion

    #region Constructor

    public TextureView( IS3DFile file )
    {
      InitializeComponent();
      _file = file;

      DataContext = ViewModel = new TextureViewViewModel();
      ViewModel.Name = file.Name;
    }

    #endregion

    #region Public Methods

    public Task Initialize( ContentHost host )
    {
      Host = host;

      return AppManager.PerformWork( host, prog =>
      {
        prog.IsIndeterminate = true;

        prog.Header = "Deserializing Texture";
        var pict = DeserializePicture( _file );
        ViewModel.ApplyMetadata( pict );

        prog.Header = "Generating Texture Previews";
        foreach ( var imageModel in TextureConverter.ConvertToPreviewModel( pict ) )
          Dispatcher.Invoke( () => ViewModel.Images.Add( imageModel ) );

        ViewModel.SelectedTexture = ViewModel.Images[ 0 ];
      } );
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

  [AddINotifyPropertyChangedInterface]
  public class TextureViewViewModel
  {

    public TextureImageModel SelectedTexture { get; set; }

    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Depth { get; set; }
    public int Faces { get; set; }
    public int MipMapCount { get; set; }
    public ObservableCollection<TextureImageModel> Images { get; }

    public TextureViewViewModel()
    {
      Images = new ObservableCollection<TextureImageModel>();
    }

    internal void ApplyMetadata( S3DPicture pict )
    {
      Width = pict.Width;
      Height = pict.Height;
      Depth = pict.Depth;
      Faces = pict.Faces;
      MipMapCount = pict.MipMapCount;
    }

  }

}
