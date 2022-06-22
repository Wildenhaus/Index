using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using H2AIndex.ViewModels;
using H2AIndex.ViewModels.Abstract;

namespace H2AIndex.Views
{

  public abstract class ViewBase : ContentControl, IView
  {

    #region Data Members

    private bool _isDisposed;

    #endregion

    #region Properties

    protected IServiceProvider ServiceProvider { get; }

    public virtual string ViewName
    {
      get
      {
        var name = GetType().Name.Replace( "View", "" );
        return Regex.Replace( name, "(\\B[A-Z])", " $1" );
      }
    }

    #endregion

    #region Constructor

    protected ViewBase()
    {
      SetResourceReference( TemplateProperty, "Template.ViewShell" );
      ServiceProvider = ( ( App ) App.Current ).ServiceProvider;

      App.Current.Exit += OnAppExit;
    }

    ~ViewBase()
    {
      Dispose( false );
    }

    #endregion

    #region IDisposable Methods

    public void Dispose()
    {
      Dispose( true );
    }

    private void Dispose( bool disposing )
    {
      if ( _isDisposed )
        return;

      if ( disposing )
        OnDisposing();

      if ( DataContext is IDisposeWithView viewModel )
        viewModel?.Dispose();

      _isDisposed = true;
    }

    protected virtual void OnDisposing()
    {
    }

    #endregion

    #region Event Handlers

    private void OnAppExit( object sender, System.Windows.ExitEventArgs e )
    {
      App.Current.Exit -= OnAppExit;
      Dispose();
    }

    #endregion

  }

  public abstract class View<TViewModel> : ViewBase, IView<TViewModel>
    where TViewModel : IViewModel
  {

    #region Properties

    public TViewModel ViewModel { get; }

    #endregion

    #region Constructor

    protected View()
    {
      //ViewModel = ServiceProvider.GetService<TViewModel>();
      //DataContext = ViewModel;

      //ViewModel.Initialize();
    }

    #endregion

  }

  public abstract class View : View<DefaultViewModel>
  {
  }

}
