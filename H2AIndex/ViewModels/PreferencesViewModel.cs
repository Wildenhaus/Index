using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2AIndex.Common.Enumerations;
using H2AIndex.Models;
using HelixToolkit.SharpDX.Core;

namespace H2AIndex.ViewModels
{

  public class PreferencesViewModel : ViewModel
  {

    #region Properties

    public PreferencesModel Preferences { get; set; }

    public IReadOnlyList<FXAALevel> RenderFxaaLevels { get; set; }
    public IReadOnlyList<ModelFileFormat> ModelFileFormats { get; set; }
    public IReadOnlyList<TextureFileFormat> TextureFileFormats { get; set; }
    public IReadOnlyList<NormalMapFormat> NormalMapFormats { get; set; }

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
      ModelFileFormats = Enum.GetValues<ModelFileFormat>();
      TextureFileFormats = Enum.GetValues<TextureFileFormat>();
      NormalMapFormats = Enum.GetValues<NormalMapFormat>();
    }

    #endregion

  }

}
