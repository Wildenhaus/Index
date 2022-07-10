using System;
using System.Threading.Tasks;
using H2AIndex.Models;

namespace H2AIndex.ViewModels
{

  public class PreferencesViewModel : ViewModel
  {

    #region Properties

    public PreferencesModel Preferences { get; set; }

    #endregion

    #region Constructor

    public PreferencesViewModel( IServiceProvider serviceProvider )
      : base( serviceProvider )
    {
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializing()
    {
      Preferences = GetPreferences();
    }

    #endregion

  }

}
