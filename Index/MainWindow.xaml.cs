using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Index.Controls;
using Index.Tools;
using Saber3D.Files;
using Saber3D.Serializers;

namespace Index
{

  public partial class MainWindow : Window
  {

    public MainWindow()
    {
      AppDaemon.SetMainWindow( this );

      InitializeComponent();
      SetIcon();

      H2AFileContext.Global.OpenFile( @"G:\h2a\re files\masterchief__h.tpl" );
      var file = H2AFileContext.Global.Files.Values.First();

      var stream = file.GetStream();
      var reader = new BinaryReader( stream );
      var tpl = new S3DTemplateSerializer().Deserialize( reader );
      var scene = SceneExporter.CreateScene( file.Name, tpl.GeometryGraph, reader );

      AppDaemon.AddEditorTab( new ModelViewerControl( scene ), file.Name );
    }

    private void SetIcon()
    {
      var assembly = Assembly.GetExecutingAssembly();
      using ( var iconStream = assembly.GetManifestResourceStream( "Index.Index.ico" ) )
        Icon = BitmapFrame.Create( iconStream );
    }

    private async void OnOpenDirectoryClick( object sender, RoutedEventArgs e )
    {
      using ( var dialog = new FolderBrowserDialog() )
      {
        dialog.Description = "Open H2A Directory";

        if ( dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK )
          return;

        var files = Directory.GetFiles( dialog.SelectedPath, "*.pck", SearchOption.AllDirectories );
        if ( files.Length == 0 )
          return;

        using ( var vm = AppDaemon.ShowLoadingView( "Loading H2A Directory" ) )
        {
          await Task.Factory.StartNew( () =>
          {

            vm.IsIndeterminate = false;
            vm.TotalUnits = files.Length;

            foreach ( var file in files )
            {
              vm.Title = $"Loading {Path.GetFileName( file )}";
              H2AFileContext.Global.OpenFile( file );
              vm.UnitsCompleted++;
            }

            vm.Title = "Organizing Files";
            FileTree.Refresh();
          }, TaskCreationOptions.LongRunning );
        }
      }
    }

    public void ToggleContentBlur()
    {
      Dispatcher.Invoke( () =>
      {
        RegisterName( "ContentBlur", ContentBlur );
        var anim = new DoubleAnimation()
        {
          From = ContentBlur.Radius > 0 ? 5 : 0,
          To = ContentBlur.Radius > 0 ? 0 : 5,
          FillBehavior = FillBehavior.HoldEnd,
          Duration = TimeSpan.FromSeconds( 0.10 )
        };

        var storyboard = new Storyboard();
        storyboard.Children.Add( anim );

        Storyboard.SetTargetName( anim, "ContentBlur" );
        Storyboard.SetTargetProperty( anim, new PropertyPath( BlurEffect.RadiusProperty ) );

        storyboard.Begin( this );
      } );
    }

  }

}
