using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using Index.ViewModels;
using Saber3D.Files;

namespace Index
{

  public partial class MainWindow : Window
  {

    public MainWindow()
    {
      AppManager.SetMainWindow( this );
      InitializeComponent();
      SetIcon();

      //H2AFileContext.Global.OpenFile( @"G:\h2a\re files\masterchief__h.tpl" );
      //var file = H2AFileContext.Global.Files.Values.First();
      //
      //var stream = file.GetStream();
      //var reader = new BinaryReader( stream );
      //var tpl = new S3DTemplateSerializer().Deserialize( reader );
      //var scene = SceneExporter.CreateScene( file.Name, tpl.GeometryGraph, reader );

      //AppDaemon.AddEditorTab( new ModelViewerControl( scene ), file.Name );

      H2AFileContext.Global.OpenFile( @"G:\h2a\d\shared\_textures_\ch_masterchief_chest.pct" );
      var file = H2AFileContext.Global.Files.Values.First();

      AppManager.CreateViewForFile( file );
    }

    private void SetIcon()
    {
      var assembly = Assembly.GetExecutingAssembly();
      using ( var iconStream = assembly.GetManifestResourceStream( "Index.Index.ico" ) )
        Icon = BitmapFrame.Create( iconStream );
    }

    #region Event Handlers

    private void OnOpenDirectoryClick( object sender, RoutedEventArgs e )
    {
      var path = AppManager.BrowseForDirectory( "Open H2A Directory" );

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

      AppManager.PerformWork( ContentHost, vm => OpenH2ADirectoryFiles( files, vm ) );
    }

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
