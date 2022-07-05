﻿using System;
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
        return GetType().Name.Replace( "View", "" );
      }
    }

    #endregion

    #region Constructor

    protected ViewBase()
    {
      SetResourceReference( TemplateProperty, "Template.ViewShell" );
      ServiceProvider = ( ( App ) App.Current ).ServiceProvider;
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
