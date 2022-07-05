using System;
using H2AIndex.ViewModels;
using H2AIndex.Views;

namespace H2AIndex.Services.Abstract
{

  public interface IViewService
  {

    IView GetView( IViewModel viewModel );

    IView GetViewWithDefaultViewModel( Type viewType );

  }

}
