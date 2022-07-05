using H2AIndex.ViewModels;

namespace H2AIndex.UI.Modals
{

  public abstract class BoundModal<TViewModel> : Modal
    where TViewModel : IViewModel
  {

    #region Properties

    public TViewModel ViewModel { get; }

    #endregion

    #region Constructor

    protected BoundModal( TViewModel viewModel )
    {
      ViewModel = viewModel;
      DataContext = ViewModel;
    }

    #endregion

  }

}
