using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Index.Modals;
using Index.UI.Controls;
using Index.ViewModels;
using Index.Views;
using Saber3D.Files;

namespace Index
{

  public static class AppManager
  {

    private static MainWindow _mainWindow;

    public static ContentHostTab AddEditorTab( string tabName, View view )
    {
      var tab = new ContentHostTab( tabName, view );

      _mainWindow.Tabs.Items.Add( tab );
      _mainWindow.Tabs.SelectedIndex = _mainWindow.Tabs.Items.Count - 1;

      return tab;
    }

    public static string BrowseForDirectory( string description = null )
    {
      using ( var dialog = new System.Windows.Forms.FolderBrowserDialog() )
      {
        if ( !string.IsNullOrWhiteSpace( description ) )
          dialog.Description = description;

        if ( dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK )
          return null;

        return dialog.SelectedPath;
      }
    }

    public static Task CreateViewForFile( IS3DFile file )
    {
      View view = null;
      switch ( file.Extension )
      {
        case ".pct":
          view = new TextureView( file );
          break;
        case ".tpl":
        case ".lg":
          view = new ModelView( file );
          break;
        case ".td":
          view = new TextEditorView( file );
          break;
      }

      if ( view == null )
        return ShowMessageModal( _mainWindow.ContentHost, "Unsupported File", "We can't open that file type yet." );

      var tab = AddEditorTab( file.Name, view );
      return view.Initialize( tab.ContentHost );
    }

    public static void ForceGarbageCollection()
    {
      // WPF has odd GC behavior when it comes to bitmaps and some other
      // resource types. To actually clear them from memory, you need to
      // collect twice, while waiting for the finalizers in between collects.
      // Yes, I know this is hacky.
      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();
    }

    public static Task PerformWork( ContentHost host, Action<ProgressViewModel> doWork )
      => PerformWork( host, vm => Task.Factory.StartNew( () => doWork( vm ), TaskCreationOptions.LongRunning ) );

    public static Task PerformWork( ContentHost host, Func<ProgressViewModel, Task> doWork )
    {
      return Task.Factory.StartNew( async () =>
      {
        // Create the modal on the UI thread
        LoadingModal loadingModal = null;
        host.Dispatcher.Invoke( () =>
        {
          loadingModal = new LoadingModal( host );
          host.Push( loadingModal );
        } );

        using ( loadingModal )
          await doWork( loadingModal.ViewModel );

      }, TaskCreationOptions.LongRunning );
    }

    public static void ShowModal<TModal>( ContentHost host )
      where TModal : HostedModal
    {
      host.Dispatcher.Invoke( () =>
      {
        var modal = ( TModal ) Activator.CreateInstance( typeof( TModal ), new object[] { host } );
        host.Push( modal );
      } );
    }

    public static void SetMainWindow( MainWindow mainWindow )
      => _mainWindow = mainWindow;

    public static Task<string> ShowMessageModal( ContentHost host, string title, string message,
      IEnumerable<string> buttons = null )
    {
      MessageModal messageModal = null;
      host.Dispatcher.Invoke( () =>
      {
        messageModal = new MessageModal( host, title, message, buttons );
        host.Push( messageModal );
      } );

      return messageModal.AwaiterTask;
    }

  }

}
