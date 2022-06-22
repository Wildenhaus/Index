using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using H2AIndex.Common;
using H2AIndex.Models;
using H2AIndex.Processes;
using H2AIndex.Services;
using H2AIndex.ViewModels.Abstract;
using H2AIndex.Views;
using Microsoft.Extensions.DependencyInjection;
using Saber3D.Files;
using Saber3D.Files.FileTypes;

namespace H2AIndex.ViewModels
{

  [AcceptsFileType( typeof( PictureFile ) )]
  public class TextureViewModel : ViewModel, IDisposeWithView
  {

    #region Data Members

    private readonly IH2AFileContext _fileContext;
    private readonly ITextureConversionService _textureService;
    private readonly ITabService _tabService;

    private readonly PictureFile _file;

    #endregion

    #region Properties

    public TextureModel Texture { get; set; }

    public ICommand OpenTextureDefinitionCommand { get; }
    public ICommand ExportTextureCommand { get; }

    #endregion

    #region Constructor

    public TextureViewModel( IServiceProvider serviceProvider, PictureFile file )
      : base( serviceProvider )
    {
      _file = file;

      _fileContext = ServiceProvider.GetRequiredService<IH2AFileContext>();
      _textureService = ServiceProvider.GetService<ITextureConversionService>();
      _tabService = ServiceProvider.GetService<ITabService>();

      OpenTextureDefinitionCommand = new AsyncCommand( OpenTextureDefinitionFile );
      ExportTextureCommand = new AsyncCommand( ExportTexture );
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializing()
    {
      using ( var progress = ShowProgress() )
      {
        progress.IsIndeterminate = true;
        progress.Status = "Loading Texture";

        var previewQuality = GetPreferences().TextureViewerOptions.PreviewQuality;
        Texture = await _textureService.LoadTexture( _file, previewQuality );
      }
    }

    protected override void OnDisposing()
    {
      Texture?.Dispose();
      base.OnDisposing();

      GCHelper.ForceCollect();
    }

    #endregion

    #region Private Methods

    private Task OpenTextureDefinitionFile()
    {
      var tdFile = _fileContext.GetFile( Path.ChangeExtension( _file.Name, ".td" ) );
      if ( tdFile is null )
        return ShowMessageModal( "File Not Found", "Could not find a texture definition for this file." );

      _tabService.CreateTabForFile( tdFile, out _ );
      return Task.CompletedTask;
    }

    private async Task ExportTexture()
    {
      var result = await ShowViewModal<TextureExportOptionsView>();
      if ( !( result is TextureExportOptionsModel options ) )
        return;

      var exportProcess = new ExportTextureProcess( _file, options );
      await RunProcess( exportProcess );
    }

    #endregion

  }

}
