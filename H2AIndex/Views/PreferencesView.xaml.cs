using H2AIndex.ViewModels;

namespace H2AIndex.Views
{

  public partial class PreferencesView : View<PreferencesViewModel>
  {

    public PreferencesView()
    {
      InitializeComponent();
    }

    protected override void OnDisposing()
    {
      ViewModel?.Dispose();
    }

  }

}
