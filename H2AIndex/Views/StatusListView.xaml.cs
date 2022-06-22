using H2AIndex.ViewModels;

namespace H2AIndex.Views
{

  public partial class StatusListView : View<StatusListViewModel>
  {

    public override string ViewName
    {
      get => "Process Results";
    }

    public StatusListView()
    {
      InitializeComponent();
    }

  }

}
