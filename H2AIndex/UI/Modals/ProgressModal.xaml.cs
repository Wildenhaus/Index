using H2AIndex.ViewModels;

namespace H2AIndex.UI.Modals
{

  public partial class ProgressModal : BoundModal<ProgressViewModel>
  {

    #region Constructor

    public ProgressModal( ProgressViewModel viewModel )
      : base( viewModel )
    {
      InitializeComponent();
    }

    #endregion

  }

}
