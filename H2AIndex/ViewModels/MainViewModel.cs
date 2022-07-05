using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using H2AIndex.Common;
using H2AIndex.Models;
using H2AIndex.Processes;
using H2AIndex.Services.Abstract;
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

      _tabService.CreateTabForFile( H2AFileContext.Global.GetFiles( ".tpl" ).First(), out _ );
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
        title: "Open Directory" ); // TODO: Add default path

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

    #endregion

  }

}
