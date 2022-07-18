using System.ComponentModel;
using H2AIndex.Common;
using PropertyChanged;

namespace H2AIndex.Models
{

  public class PreferencesModel : ObservableObject
  {

    #region Properties

    public static PreferencesModel Default
    {
      get
      {
        return new PreferencesModel
        {
          ModelExportOptions = ModelExportOptionsModel.Default,
          ModelViewerOptions = ModelViewerOptionsModel.Default,
          TextureExportOptions = TextureExportOptionsModel.Default,
          TextureViewerOptions = TextureViewerOptionsModel.Default
        };
      }
    }

    [DefaultValue( false )]
    public bool LoadH2ADirectoryOnStartup { get; set; }

    public string H2ADirectoryPath { get; set; }

    [OnChangedMethod( nameof( SetGlobalDefaults ) )]
    public string DefaultExportPath { get; set; }

    [OnChangedMethod( nameof( SetGlobalDefaults ) )]
    public ModelExportOptionsModel ModelExportOptions { get; set; }

    [OnChangedMethod( nameof( SetGlobalDefaults ) )]
    public TextureExportOptionsModel TextureExportOptions { get; set; }

    public ModelViewerOptionsModel ModelViewerOptions { get; set; }
    public TextureViewerOptionsModel TextureViewerOptions { get; set; }

    #endregion

    #region Constructor

    public PreferencesModel()
    {
    }

    #endregion

    #region Private Methods

    private void SetGlobalDefaults()
    {
      if ( ModelExportOptions != null )
        ModelExportOptions.OutputPath = DefaultExportPath;
      if ( TextureExportOptions != null )
        TextureExportOptions.OutputPath = DefaultExportPath;
    }

    #endregion

  }

}
