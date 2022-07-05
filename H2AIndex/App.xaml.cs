using System;
using System.Windows;
using H2AIndex.Processes;
using H2AIndex.Services;
using H2AIndex.Services.Abstract;
using H2AIndex.UI.Modals;
using H2AIndex.ViewModels;
using H2AIndex.Views;
using Microsoft.Extensions.DependencyInjection;

namespace H2AIndex
{

  public partial class App : Application
  {

    #region Data Members

    private IServiceProvider _serviceProvider;

    #endregion

    #region Properties

    public IServiceProvider ServiceProvider
    {
      get => _serviceProvider;
    }

    public MainViewModel MainViewModel { get; private set; }

    #endregion

    #region Constructor

    public App()
    {
    }

    #endregion

    #region Private Methods

    private void ConfigureDependencies( IServiceCollection services )
    {
      ConfigureModals( services );
      ConfigureViews( services );
      ConfigureViewModels( services );
      ConfigureWindows( services );

      ConfigureProcesses( services );
      ConfigureServices( services );
    }

    private void ConfigureModals( IServiceCollection services )
    {
      services.AddTransient<MessageModal>();
      services.AddTransient<ProgressModal>();
    }

    private void ConfigureViews( IServiceCollection services )
    {
      services.AddTransient<MainView>();

      services.AddTransient<AboutView>();
      services.AddTransient<ModelView>();
      services.AddTransient<TextureView>();
    }

    private void ConfigureViewModels( IServiceCollection services )
    {
      services.AddSingleton<MainViewModel>();
      services.AddTransient<DefaultViewModel>();

      services.AddTransient<AboutViewModel>();
      services.AddTransient<ModelViewModel>();
      services.AddTransient<ProgressViewModel>();
      services.AddTransient<TextureViewModel>();
    }

    private void ConfigureWindows( IServiceCollection services )
    {
      services.AddTransient<MainWindow>();
    }

    private void ConfigureProcesses( IServiceCollection services )
    {
      services.AddTransient<OpenFilesProcess>();
    }

    private void ConfigureServices( IServiceCollection services )
    {
      services.AddTransient<IFileDialogService, FileDialogService>();
      services.AddTransient<ITextureConversionService, TextureConversionService>();
      services.AddTransient<ITabService, TabService>();
      services.AddTransient<IViewService, ViewService>();
    }

    #endregion

    #region Event Handlers

    private void OnAppStartup( object sender, StartupEventArgs e )
    {
      var services = new ServiceCollection();
      ConfigureDependencies( services );
      _serviceProvider = services.BuildServiceProvider();

      var window = _serviceProvider.GetService<MainWindow>();
      MainViewModel = ( MainViewModel ) window.DataContext;

      window.Show();
    }

    #endregion

  }

}
