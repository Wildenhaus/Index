using Index.Controls;
using Index.ViewModels;

namespace Index.Modals
{

  public partial class LoadingModal : HostedModal
  {

    #region Properties

    public ProgressViewModel ViewModel { get; }

    #endregion

    #region Constructor

    public LoadingModal( ContentHost host )
      : base( host )
    {
      InitializeComponent();
      DataContext = ViewModel = new ProgressViewModel();
    }

    #endregion

  }

}
