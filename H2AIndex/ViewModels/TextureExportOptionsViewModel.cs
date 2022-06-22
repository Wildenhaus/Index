using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using H2AIndex.Common.Enumerations;
using H2AIndex.Models;
using H2AIndex.UI.Modals;

namespace H2AIndex.ViewModels
{

  public class TextureExportOptionsViewModel : ViewModel, IModalFooterButtons
  {

    #region Properties

    public IReadOnlyList<TextureFileFormat> TextureFileFormats { get; set; }
    public IReadOnlyList<NormalMapFormat> NormalMapFormats { get; set; }

    public bool IsForBatch { get; set; }
    public TextureExportOptionsModel Options { get; set; }

    public bool IsValidPath { get; set; }

    #endregion

    #region Constructor

    public TextureExportOptionsViewModel( IServiceProvider serviceProvider )
      : base( serviceProvider )
    {
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializing()
    {
      TextureFileFormats = Enum.GetValues<TextureFileFormat>();
      NormalMapFormats = Enum.GetValues<NormalMapFormat>();

      Options = GetPreferences().TextureExportOptions;

      var defaultExportPath = GetPreferences().DefaultExportPath;
      if ( string.IsNullOrWhiteSpace( defaultExportPath ) && Directory.Exists( defaultExportPath ) )
        Options.OutputPath = defaultExportPath;
    }

    #endregion

    #region IModalFooterButtons Members

    public IEnumerable<Button> GetFooterButtons()
    {
      yield return new Button { Content = "Cancel" };

      var exportBtn = new Button
      {
        Content = "Export",
        Style = ( Style ) App.Current.FindResource( "ColorfulFooterButtonStyle" ),
        CommandParameter = Options
      };

      var exportBtnEnabledBinding = new Binding( nameof( IsValidPath ) );
      exportBtnEnabledBinding.Source = this;
      BindingOperations.SetBinding( exportBtn, Button.IsEnabledProperty, exportBtnEnabledBinding );

      yield return exportBtn;
    }

    #endregion

  }

}
