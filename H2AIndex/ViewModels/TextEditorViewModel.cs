using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using H2AIndex.Common;
using H2AIndex.Services;
using H2AIndex.ViewModels.Abstract;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Extensions.DependencyInjection;
using Saber3D.Files;
using Saber3D.Files.FileTypes;

namespace H2AIndex.ViewModels
{

  [AcceptsFileType( typeof( GenericTextFile ) )]
  [AcceptsFileType( typeof( TextureDefinitionFile ) )]
  public class TextEditorViewModel : ViewModel, IDisposeWithView
  {

    #region Data Members

    private readonly IS3DFile _file;
    private readonly IFileDialogService _dialogService;

    #endregion

    #region Properties

    public TextDocument Document { get; set; }

    public ICommand ExportCommand { get; }

    #endregion

    #region Constructor

    public TextEditorViewModel( IServiceProvider serviceProvider, IS3DFile file )
      : base( serviceProvider )
    {
      _file = file;
      _dialogService = serviceProvider.GetRequiredService<IFileDialogService>();

      ExportCommand = new AsyncCommand( ExportFile );
    }

    #endregion

    #region Overides

    protected override async Task OnInitializing()
    {
      try
      {
        Document = await LoadDocument( _file );
      }
      catch ( Exception ex )
      {
        await ShowExceptionModal( ex );
      }
    }

    #endregion

    #region Private Methods

    private async Task<TextDocument> LoadDocument( IS3DFile file )
    {
      var fileStream = file.GetStream();

      try
      {
        fileStream.AcquireLock();
        using ( var reader = new StreamReader( fileStream, leaveOpen: true ) )
          return new TextDocument( await reader.ReadToEndAsync() );
      }
      finally { fileStream.ReleaseLock(); }
    }

    private async Task ExportFile()
    {
      var fName = _file.Name;
      var path = GetPreferences().DefaultExportPath;
      if ( !string.IsNullOrWhiteSpace( path ) && Directory.Exists( path ) )
        fName = Path.Combine( path, fName );

      var outputFile = await _dialogService.BrowseForSaveFile(
        title: "Export File",
        defaultFileName: fName,
        filter: $"Saber File|*{Path.GetExtension( fName )}" );

      if ( string.IsNullOrWhiteSpace( outputFile ) )
        return;

      using ( var fs = File.Create( outputFile ) )
      using ( var writer = new StreamWriter( fs ) )
      {
        Document.WriteTextTo( writer );

        await writer.FlushAsync();
      }
    }

    #endregion

  }

}
