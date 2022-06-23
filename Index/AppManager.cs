using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Index.Controls;
using Index.Modals;
using Index.ViewModels;
using Index.Views;
using Saber3D.Files;

namespace Index
{

  public static class AppManager
  {

    private static MainWindow _mainWindow;

    public static ContentHost AddEditorTab( UIElement control, string tabName )
    {
      var host = new ContentHost();
      host.Children.Add( control );

      var tab = new TabItem
      {
        Header = tabName,
        Content = host
      };

      _mainWindow.Tabs.Items.Add( tab );
      _mainWindow.Tabs.SelectedIndex = _mainWindow.Tabs.Items.Count - 1;

      return host;
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
      UIElement view = null;
      switch ( file.Extension )
      {
        case ".pct":
          view = new TextureView( file );
          break;
      }

      if ( view == null )
        return ShowMessageModal( _mainWindow.ContentHost, "Unsupported File", "We can't open that file type yet." );

      var tabHost = AddEditorTab( view, file.Name );
      if ( view is IInitializableView initializableView )
        return initializableView.Initialize( tabHost );

      return Task.CompletedTask;
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
