using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using H2AIndex.Common;
using H2AIndex.Models;
using H2AIndex.Processes;
using H2AIndex.Services;
using H2AIndex.Views;
using Microsoft.Extensions.DependencyInjection;
using Saber3D.Files;

namespace H2AIndex.ViewModels
{

  public class MainViewModel : ViewModel
  {

    #region Data Members

    private readonly ITabService _tabService;

    #endregion

    #region Properties

    public FileContextModel FileContext { get; }

    public TabContextModel TabContext { get; }
    public ICommand OpenFileTabCommand { get; }

    public ICommand OpenFileCommand { get; }
    public ICommand OpenDirectoryCommand { get; }
    public ICommand ExitCommand { get; }

    public ICommand BulkExportTexturesCommand { get; }

    #endregion

    #region Constructor

    public MainViewModel( IServiceProvider serviceProvider )
      : base( serviceProvider )
    {
      FileContext = new FileContextModel();

      _tabService = serviceProvider.GetService<ITabService>();
      TabContext = _tabService.TabContext;
      OpenFileTabCommand = new AsyncCommand<FileModel>( OpenFileTab );

      OpenFileCommand = new AsyncCommand( OpenFile );
      OpenDirectoryCommand = new AsyncCommand( OpenDirectory );

      BulkExportTexturesCommand = new AsyncCommand( BulkExportTextures );

      App.Current.DispatcherUnhandledException += OnUnhandledExceptionRaised;
    }

    protected override Task OnInitializing()
    {
      var sl = new StatusList();
      sl.AddMessage( "message name", "this is a message" );
      sl.AddWarning( "warning name", "this is a warning" );
      sl.AddError( "error name", "this is an error", new Exception( "Exception message" ) );
      sl.AddError( "error name 2", new Exception( "Exception message 2" ) );

      ShowStatusListModal( sl );
      return Task.CompletedTask;
    }

    #endregion

    #region Private Methods

    private async Task OpenFile()
    {
      var filePaths = await ShowOpenFileDialog(
        title: "Open File" ); // TODO: Add filter

      if ( filePaths == null )
        return;

      var process = new OpenFilesProcess( H2AFileContext.Global, filePaths );
      await RunProcess( process );
    }

    private async Task OpenDirectory()
    {
      var directoryPath = await ShowFolderBrowserDialog(
        title: "Open Directory",
        defaultPath: GetPreferences().H2ADirectoryPath );

      if ( directoryPath == null )
        return;

      var process = new OpenFilesProcess( H2AFileContext.Global, directoryPath );
      await RunProcess( process );
    }

    private async Task OpenFileTab( FileModel fileModel )
    {
      var file = fileModel.File;
      if ( !_tabService.CreateTabForFile( file, out _ ) )
      {
        var fileExt = Path.GetExtension( file.Name );
        await ShowMessageModal(
          title: "Unsupported File Type",
          message: $"We can't open {fileExt} files yet." );

        return;
      }
    }

    private async Task BulkExportTextures()
    {
      var result = await ShowViewModal<TextureExportOptionsView>( vm =>
      {
        var viewModel = vm as TextureExportOptionsViewModel;
        viewModel.IsForBatch = true;
      } );

      if ( !( result is TextureExportOptionsModel options ) )
        return;

      var exportProcess = new BulkExportTexturesProcess( options );
      await RunProcess( exportProcess );
    }

    #endregion

    #region Event Handlers

    private async void OnUnhandledExceptionRaised( object sender, DispatcherUnhandledExceptionEventArgs e )
    {
      await ShowExceptionModal( e.Exception );
    }

    #endregion

  }

}
