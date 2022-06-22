using System.Windows.Controls;
using Index.Controls;
using Index.ViewModels;

namespace Index
{

  public static class AppDaemon
  {

    private static MainWindow _mainWindow;

    public static void SetMainWindow( MainWindow mainWindow )
      => _mainWindow = mainWindow;

    public static LoadingViewModel ShowLoadingView( string title = "Loading" )
    {
      LoadingControl loadingControl = null;
      LoadingViewModel viewModel = null;

      _mainWindow.Dispatcher.Invoke( () =>
      {
        loadingControl = new LoadingControl();
        Grid.SetColumnSpan( loadingControl, 2 );
        Grid.SetZIndex( loadingControl, 99 );

        viewModel = new LoadingViewModel();
        viewModel.Title = title;

        loadingControl.DataContext = viewModel;

        viewModel.Disposing += ( s, e ) =>
        {
          loadingControl.Hide().ContinueWith( t =>
          {
            _mainWindow.ToggleContentBlur();
            _mainWindow.Dispatcher.Invoke( () =>
            {
              _mainWindow.ContentHost.Children.Remove( loadingControl );
            } );
          } );
        };

        _mainWindow.ContentHost.Children.Add( loadingControl );
        _mainWindow.ToggleContentBlur();
        loadingControl.Show();
      } );

      return viewModel;
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
