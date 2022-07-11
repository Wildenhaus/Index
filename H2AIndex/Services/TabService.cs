using System;
using System.IO;
using System.Linq;
using H2AIndex.Models;
using H2AIndex.ViewModels;
using Saber3D.Files;

namespace H2AIndex.Services
{

  public class TabService : ITabService
  {

    #region Data Members

    private readonly IServiceProvider _serviceProvider;
    private readonly IFileTypeService _fileTypeService;
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

    public TabService( IServiceProvider serviceProvider,
      IFileTypeService fileTypeService,
      IViewService viewService )
    {
      _serviceProvider = serviceProvider;
      _fileTypeService = fileTypeService;
      _viewService = viewService;
      _tabContext = new TabContextModel();
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

      var viewModelType = _fileTypeService.GetViewModelType( file.GetType() );
      if ( viewModelType is null )
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

    #endregion

  }

}
