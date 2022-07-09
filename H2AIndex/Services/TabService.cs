using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using H2AIndex.Common;
using H2AIndex.Models;
using H2AIndex.ViewModels;
using Saber3D.Files;

namespace H2AIndex.Services
{

  public class TabService : ITabService
  {

    #region Data Members

    // Dictionary<IS3DFileType, ViewModelType>
    private static readonly Dictionary<Type, Type> _viewModelLookup;

    private readonly IServiceProvider _serviceProvider;
    private readonly IViewService _viewService;

    private readonly TabContextModel _tabContext;

    #endregion

    #region Properties

    public TabContextModel TabContext
    {
      get => _tabContext;
    }

    #endregion

    #region Constructor

    public TabService( IServiceProvider serviceProvider, IViewService viewService )
    {
      _serviceProvider = serviceProvider;
      _viewService = viewService;
      _tabContext = new TabContextModel();
    }

    static TabService()
    {
      _viewModelLookup = BuildViewModelLookup();
    }

    #endregion

    #region Public Methods

    public bool CreateTabForFile( IS3DFile file, out ITab tab )
    {
      tab = default;

      if ( NavigateToTab( file.Name ) )
      {
        tab = _tabContext.CurrentTab;
        return true;
      }

      if ( !_viewModelLookup.TryGetValue( file.GetType(), out var viewModelType ) )
        return false;

      var viewModel = ( IViewModel ) Activator.CreateInstance( viewModelType, new object[] { _serviceProvider, file } );
      viewModel.Initialize();

      var view = _viewService.GetView( viewModel );

      var fileName = Path.GetFileName( file.Name );
      tab = new Tab( fileName, view );

      _tabContext.AddTab( tab );

      return true;
    }

    public bool NavigateToTab( string tabName )
    {
      if ( !TryFindTab( tabName, out var tab ) )
        return false;

      _tabContext.CurrentTab = tab;
      return true;
    }

    #endregion

    #region Private Methods

    private bool TryFindTab( string tabName, out ITab tab )
    {
      tab = _tabContext.Tabs.FirstOrDefault( x => x.Name == tabName );
      return tab != null;
    }

    private static Dictionary<Type, Type> BuildViewModelLookup()
    {
      var lookup = new Dictionary<Type, Type>();

      var viewModelTypes = GetViewModelTypes();
      foreach ( var viewModelType in viewModelTypes )
      {
        var attributes = viewModelType.GetCustomAttributes( typeof( AcceptsFileTypeAttribute ), true )
          .Cast<AcceptsFileTypeAttribute>();

        foreach ( var attribute in attributes )
          lookup.Add( attribute.FileType, viewModelType );
      }

      return lookup;
    }

    private static IEnumerable<Type> GetViewModelTypes()
    {
      return typeof( TabService ).Assembly.GetTypes()
        .Where( x =>
        {
          if ( !x.IsClass || x.IsAbstract )
            return false;

          if ( !typeof( IViewModel ).IsAssignableFrom( x ) )
            return false;

          return true;
        } );
    }

    #endregion

  }

}
