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

    public ICommand EditPreferencesCommand { get; }

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
      EditPreferencesCommand = new AsyncCommand( EditPreferences );

      BulkExportTexturesCommand = new AsyncCommand( BulkExportTextures );

      App.Current.DispatcherUnhandledException += OnUnhandledExceptionRaised;

      //_tabService.CreateTabForFile( H2AFileContext.Global.GetFiles( "_h.tpl" ).First(), out _ );
      //_tabService.CreateTabForFile( H2AFileContext.Global.GetFiles( ".lg" ).First(), out _ );
      //var p = GetPreferences();
      //H2AFileContext.Global.OpenFile( @"F:\cortana__h.tpl" );
      //var file = H2AFileContext.Global.GetFile( "cortana__h.tpl" );

      //_tabService.CreateTabForFile( file, out _ );

      //H2AFileContext.Global.OpenFile( @"F:\brute__h.tpl" );
      //var file = H2AFileContext.Global.GetFile( "brute__h.tpl" );

      //_tabService.CreateTabForFile( file, out _ );

      //H2AFileContext.Global.OpenFile( @"G:\h2a\re files\dervish__h.tpl" );
      //var file = H2AFileContext.Global.GetFile( "dervish__h.tpl" );
      //_tabService.CreateTabForFile( file, out _ );

      //H2AFileContext.Global.OpenFile( @"F:\warthog__h.tpl" );
      //var file = H2AFileContext.Global.GetFile( "warthog__h.tpl" );
      //_tabService.CreateTabForFile( file, out _ );

      //var proc = new ExportModelProcess( file,
      //  Tuple.Create( p.ModelExportOptions, p.TextureExportOptions ) );
      //RunProcess( proc );
    }

    #endregion

    #region Private Methods

    private async Task OpenFile()
    {
      var filePaths = await ShowOpenFileDialog(
        title: "Open File",
        initialDirectory: GetPreferences().H2ADirectoryPath ); // TODO: Add filter

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

    private async Task EditPreferences()
    {
      await ShowViewModal<PreferencesView>();
      await SavePreferences();
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
