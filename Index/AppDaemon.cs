using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using Index.Controls;
using Index.Modals;

namespace Index
{

  public static class AppDaemon
  {

    private static MainWindow _mainWindow;

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

    public static Task PerformWork( ContentHost host, Action<LoadingModalViewModel> doWork )
    {
      return Task.Factory.StartNew( () =>
      {
        // Create the modal on the UI thread
        LoadingModal loadingModal = null;
        host.Dispatcher.Invoke( () =>
        {
          loadingModal = new LoadingModal( host );
          host.Push( loadingModal );
        } );

        using ( loadingModal )
          doWork( loadingModal.ViewModel );
      }, TaskCreationOptions.LongRunning );
    }

    public static void AddEditorTab( UserControl control, string tabName )
    {
      var tab = new TabItem
      {
        Header = tabName,
        Content = control
      };

      _mainWindow.Tabs.Items.Add( tab );
    }

  }

}
