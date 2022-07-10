using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2AIndex.Models;
using HelixToolkit.SharpDX.Core;

namespace H2AIndex.ViewModels
{

  public class PreferencesViewModel : ViewModel
  {

    #region Properties

    public PreferencesModel Preferences { get; set; }

    public IReadOnlyList<FXAALevel> RenderFxaaLevels { get; set; }

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

      RenderFxaaLevels = Enum.GetValues<FXAALevel>();
    }

    #endregion

  }

}
