using System;
using System.Collections.Generic;
using System.Linq;
using H2AIndex.Services.Abstract;
using H2AIndex.ViewModels;
using H2AIndex.Views;

namespace H2AIndex.Services
{

  public class ViewService : IViewService
  {

    #region Data Members

    //Dictionary<ViewModelType, ViewType>
    private static readonly Dictionary<Type, Type> _viewLookup;

    //Dictionary<ViewType, ViewModelType>
    private static readonly Dictionary<Type, Type> _viewModelLookup;

    private IServiceProvider _serviceProvider;

    #endregion

    #region Constructor

    public ViewService( IServiceProvider serviceProvider )
    {
      _serviceProvider = serviceProvider;
    }

    static ViewService()
    {
      _viewLookup = BuildViewLookup();
      _viewModelLookup = BuildViewModelLookup();
    }

    #endregion

    #region Public Methods

    public IView GetView( IViewModel viewModel )
    {
      var viewModelType = viewModel.GetType();
      if ( !_viewLookup.TryGetValue( viewModelType, out var viewType ) )
        return null;

      var view = ( IView ) _serviceProvider.GetService( viewType );
      view.DataContext = viewModel;

      return view;
    }

    public IView GetViewWithDefaultViewModel( Type viewType )
    {
      var view = ( IView ) _serviceProvider.GetService( viewType );

      if ( !_viewModelLookup.TryGetValue( viewType, out var viewModelType ) )
        viewModelType = typeof( DefaultViewModel );

      var viewModel = ( IViewModel ) _serviceProvider.GetService( viewModelType );
      view.DataContext = viewModel;

      viewModel.Initialize();

      return view;
    }

    #endregion

    #region Private Methods

    private static Dictionary<Type, Type> BuildViewLookup()
    {
      var lookup = new Dictionary<Type, Type>();

      var viewTypes = GetViewTypes();
      foreach ( var viewType in viewTypes )
      {
        var viewModelType = viewType.BaseType.GenericTypeArguments.Single();
        lookup.Add( viewModelType, viewType );
      }

      return lookup;
    }

    private static Dictionary<Type, Type> BuildViewModelLookup()
    {
      var lookup = new Dictionary<Type, Type>();

      var viewTypes = GetViewTypes();
      foreach ( var viewType in viewTypes )
      {
        var viewModelType = viewType.BaseType.GenericTypeArguments.Single();
        lookup.Add( viewType, viewModelType );
      }

      return lookup;
    }

    private static IEnumerable<Type> GetViewTypes()
    {
      return typeof( ViewService ).Assembly.GetTypes()
        .Where( x =>
        {
          if ( !x.IsClass || x.IsAbstract )
            return false;

          if ( !typeof( IView ).IsAssignableFrom( x ) )
            return false;

          if ( x.BaseType.GenericTypeArguments.Length != 1 )
            return false;

          return true;
        } );
    }

    #endregion

  }

}
