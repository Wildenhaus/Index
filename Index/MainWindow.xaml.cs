using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using Saber3D.Files;

namespace Index
{

  public partial class MainWindow : Window
  {

    public MainWindow()
    {
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
      //using ( var dialog = new FolderBrowserDialog() )
      //{
      //  dialog.Description = "Open H2A Directory";

      //  if ( dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK )
      //    return;

      //  var files = Directory.GetFiles( dialog.SelectedPath, "*.pck", SearchOption.AllDirectories );
      //  if ( files.Length == 0 )
      //    return;
      //}


      var path = @"G:\Steam\steamapps\common\Halo The Master Chief Collection\halo1";
      var files = Directory.GetFiles( path, "*.pck", SearchOption.AllDirectories );
      if ( files.Length == 0 )
      {
        AppDaemon.ShowMessageModal( ContentHost,
          title: "No Files Found",
          message: "No Halo 2 Anniversary .pck files were found. " +
                   "If you meant to open a file that you previously extracted, " +
                   "use the 'Open File' menu item instead." );
        return;
      }

      AppDaemon.PerformWork( ContentHost, vm =>
      {
        vm.TotalUnits = files.Length;
        vm.UnitName = "files";

        foreach ( var file in files )
        {
          vm.Header = $"Loading {Path.GetFileName( file )}";
          H2AFileContext.Global.OpenFile( file );
          vm.CompletedUnits++;
        }

        vm.Header = "Organizing Files";
        vm.IsIndeterminate = true;
        FileTree.Refresh();
      } );
    }

    #endregion

  }

}
