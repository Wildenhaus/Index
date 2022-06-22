using System;
using System.Windows;
using System.Windows.Input;
using H2AIndex.Common;
using H2AIndex.ViewModels;

namespace H2AIndex
{

  public partial class MainWindow : Window
  {

    #region Data Members

    private IServiceProvider _serviceProvider;

    #endregion

    #region Properties

    public ICommand CloseWindowCommand { get; }
    public ICommand MaximizeWindowCommand { get; }
    public ICommand MinimizeWindowCommand { get; }

    #endregion

    #region Constructor

    public MainWindow( IServiceProvider serviceProvider, MainViewModel viewModel )
    {
      _serviceProvider = serviceProvider;

      CloseWindowCommand = new Command( Close );
      MaximizeWindowCommand = new Command( MaximizeWindow );
      MinimizeWindowCommand = new Command( MinimizeWindow );

      InitializeComponent();
      DataContext = viewModel;
      viewModel.Initialize();
    }

    #endregion

    #region Private Methods

    private void MinimizeWindow()
      => WindowState = WindowState.Minimized;

    private void MaximizeWindow()
    {
      if ( WindowState == WindowState.Maximized )
        WindowState = WindowState.Normal;
      else
        WindowState = WindowState.Maximized;
    }

    #endregion

  }

}
