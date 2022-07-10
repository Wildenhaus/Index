using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using H2AIndex.Common;
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

    public ICommand ExplainFileFiltersCommand { get; }
    public ICommand ExplainTextureDefinitionsCommand { get; }

    #endregion

    #region Constructor

    public TextureExportOptionsViewModel( IServiceProvider serviceProvider )
      : base( serviceProvider )
    {
      ExplainFileFiltersCommand = new AsyncCommand( ExplainFileFilters );
      ExplainTextureDefinitionsCommand = new AsyncCommand( ExplainTextureDefinitions );
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

    #region Private Methods

    private async Task ExplainFileFilters()
    {
      var message = "This textbox allows you to filter your batch export down to files that match certain criteria. " +
        "Filters are delimited by a semicolon (;) and are case-insensitive. Wildcard (*) is not supported.\n" +
        "\n" +
        "Example Usage: masterchief;dervish\n" +
        "This will only export files with 'masterchief' and 'dervish' in their names.";

      await ShowMessageModal(
        title: "File Filters",
        message: message,
        showOnMainView: true );
    }

    private async Task ExplainTextureDefinitions()
    {
      var message = "Texture Definitions are text files that describe how materials use a particular texture.\n" +
        "Little is known about how these work in the engine, but they provide important information " +
        "when setting up game-accurate shaders.";

      await ShowMessageModal(
        title: "Export Texture Definitions",
        message: message,
        showOnMainView: true );
    }

    #endregion

  }

}
