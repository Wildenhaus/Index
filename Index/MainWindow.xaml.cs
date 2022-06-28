using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Index.Common;
using Index.Modals;
using Index.ViewModels;
using Saber3D.Files;

namespace Index
{

  public partial class MainWindow : Window
  {

    public ICommand CloseWindowCommand { get; }
    public ICommand MaximizeWindowCommand { get; }
    public ICommand MinimizeWindowCommand { get; }

    public MainWindow()
    {
      AppManager.SetMainWindow( this );
      InitializeComponent();
      CloseWindowCommand = new DelegateCommand( OnCloseWindowClick );
      MinimizeWindowCommand = new DelegateCommand( OnMinimizeWindowClick );
      MaximizeWindowCommand = new DelegateCommand( OnMaximizeWindowClick );

      //OnShowAboutClick( null, null );
      //OnShowPreferencesClick( null, null );

      H2AFileContext.Global.OpenFile( @"G:\h2a\re files\masterchief__h.tpl" );
      foreach ( var texFile in Directory.GetFiles( @"G:\h2a\d\", "*.pct", SearchOption.AllDirectories ) )
        if ( texFile.Contains( "masterchief" ) )
          H2AFileContext.Global.OpenFile( texFile );

      var file = H2AFileContext.Global.GetFiles( ".tpl" ).First();
      AppManager.CreateViewForFile( file );

      //
      //var stream = file.GetStream();
      //var reader = new BinaryReader( stream );
      //var tpl = new S3DTemplateSerializer().Deserialize( reader );
      //var scene = SceneExporter.CreateScene( file.Name, tpl.GeometryGraph, reader );

      //AppDaemon.AddEditorTab( new ModelViewerControl( scene ), file.Name );

      //H2AFileContext.Global.OpenFile( @"G:\h2a\d\shared\_textures_\ch_masterchief_chest.pct" );
      //var file = H2AFileContext.Global.Files.Values.First();

      //AppManager.CreateViewForFile( file );
    }

    #region Event Handlers

    private void OnOpenDirectoryClick( object sender, RoutedEventArgs e )
    {
      var prefs = PreferencesManager.Preferences;
      var path = AppManager.BrowseForDirectory(
        title: "Open H2A Directory",
        defaultPath: prefs.H2AGameDirectory );

      if ( string.IsNullOrEmpty( path ) )
        return;

      var files = Directory.GetFiles( path, "*.pck", SearchOption.AllDirectories );
      if ( files.Length == 0 )
      {
        AppManager.ShowMessageModal( ContentHost,
          title: "No Files Found",
          message: "No Halo 2 Anniversary .pck files were found. " +
                   "If you meant to open a file that you previously extracted, " +
                   "use the 'Open File' menu item instead." );
        return;
      }

      if ( string.IsNullOrWhiteSpace( prefs.H2AGameDirectory ) )
        prefs.H2AGameDirectory = path;

      AppManager.PerformWork( ContentHost, vm => OpenH2ADirectoryFiles( files, vm ) );
    }

    private void OnCloseWindowClick( object sender )
      => Close();

    private void OnMinimizeWindowClick( object sender )
      => WindowState = WindowState.Minimized;

    private void OnMaximizeWindowClick( object sender )
    {
      if ( WindowState == WindowState.Maximized )
        WindowState = WindowState.Normal;
      else
        WindowState = WindowState.Maximized;
    }

    private void OnOpenDiscordClick( object sender, RoutedEventArgs e )
      => Process.Start( new ProcessStartInfo( "https://discord.com/invite/haloarchive" ) { UseShellExecute = true } );

    private void OnShowAboutClick( object sender, RoutedEventArgs e )
      => AppManager.ShowModal<AboutModal>( ContentHost );

    private void OnShowPreferencesClick( object sender, RoutedEventArgs e )
      => AppManager.ShowModal<PreferencesModal>( ContentHost );

    #endregion

    #region Private Methods

    private void OpenH2ADirectoryFiles( string[] files, ProgressViewModel vm )
    {
      string currentFile = null;
      try
      {
        vm.TotalUnits = files.Length;
        vm.UnitName = "file(s) loaded";

        foreach ( var file in files )
        {
          vm.Header = $"Loading {Path.GetFileName( file )}";
          H2AFileContext.Global.OpenFile( file );
          vm.CompletedUnits++;
        }

        vm.Header = "Organizing Files";
        vm.IsIndeterminate = true;
        FileTree.Refresh();
      }
      catch ( Exception ex )
      {
        AppManager.ShowMessageModal( ContentHost,
          title: "Error",
          message: "Encountered an error while attempting to load the game files:\n"
                + $"File: {currentFile}\n"
                + $"Error: {ex.Message}"
          ); ;
      }
    }


    #endregion


  }

}
