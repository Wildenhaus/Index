using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Index.Common;
using Index.Models;
using Index.Tools;
using Index.UI.Controls;
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

    public ICommand CloseTabCommand { get; }
    public ICommand OpenTextureDefinitionCommand { get; }
    public ICommand ExportCommand { get; }

    #endregion

    #region Constructor

    public TextureView( IS3DFile file )
    {
      CloseTabCommand = new DelegateCommand( CloseFile );
      OpenTextureDefinitionCommand = new DelegateCommand( OpenTextureDefinition );
      ExportCommand = new DelegateCommand( ExportTexture );
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
        var previewQuality = PreferencesManager.Preferences.TexturePreviewQuality;
        foreach ( var image in TextureConverter.ConvertToBitmaps( pict, previewQuality ) )
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

    private void CloseFile( object param )
    {
      if ( Host.Parent is ContentHostTab hostTab )
        hostTab.CloseTabCommand.Execute( param );
    }

    private void OpenTextureDefinition( object param )
    {
      var textureDefinitionName = Path.ChangeExtension( _file.Name, ".td" );
      var tdFile = H2AFileContext.Global.GetFiles( textureDefinitionName ).FirstOrDefault();
      if ( tdFile is null )
      {
        AppManager.ShowMessageModal( Host,
          "Error",
          $"Couldn't find a texture definition for this file.\n" +
          $"Please ensure that you have cache.pck loaded." );
        return;
      }

      AppManager.CreateViewForFile( tdFile );
    }

    private void ExportTexture( object param )
    {
      var format = param as string;
      if ( format == null )
        format = "DDS";

      format = format.ToLower();

      switch ( param )
      {
        case "TGA":
          ConvertAndExportTexture( format, pict => TextureConverter.ConvertToTGA( pict, 0 ) );
          break;
        case "JPG":
          var jpgExportQuality = PreferencesManager.Preferences.TextureJpegExportQuality;
          ConvertAndExportTexture( format, pict => TextureConverter.ConvertToJpg( pict, 0, jpgExportQuality ) );
          break;
        case "DDS":
        default:
          ConvertAndExportTexture( format, TextureConverter.ConvertToDDS );
          break;
      }
    }

    private void ConvertAndExportTexture( string format, Func<S3DPicture, Stream> convertFunc )
    {
      var exportFileName = Path.ChangeExtension( _file.Name, format.ToLower() );

      var exportPath = AppManager.BrowseForSaveFile(
        title: "Export Selected Texture",
        defaultFileName: exportFileName,
        filter: $"{format} Image | *.{format.ToLower()}" );

      if ( exportPath == null )
        return;

      AppManager.PerformWork( Host, prog =>
      {
        prog.IsIndeterminate = true;
        prog.Header = $"Exporting {exportFileName}";

        var pict = DeserializePicture( _file );
        using ( var convertedStream = convertFunc( pict ) )
        using ( var fileStream = File.Create( exportPath ) )
        {
          convertedStream.CopyTo( fileStream );
          fileStream.Flush();
        }
      } );
    }

    #endregion

  }

}
