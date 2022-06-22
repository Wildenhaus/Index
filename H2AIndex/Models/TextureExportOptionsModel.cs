using System.ComponentModel;
using System.Text.Json.Serialization;
using DeepCopy;
using H2AIndex.Common;
using H2AIndex.Common.Enumerations;

namespace H2AIndex.Models
{

  public class TextureExportOptionsModel : ObservableObject, IDeepCopy<TextureExportOptionsModel>
  {

    #region Data Members

    public static TextureExportOptionsModel Default
    {
      get
      {
        return new TextureExportOptionsModel
        {
          OutputFileFormat = TextureFileFormat.DDS,
          OutputNormalMapFormat = NormalMapFormat.OpenGL,
          RecalculateNormalMapZChannel = true,
          ExportAllMips = false,
          OverwriteExisting = true,
          ExportTextureDefinition = true
        };
      }
    }

    #endregion

    #region Properties

    [JsonIgnore]
    public string OutputPath { get; set; }

    [JsonIgnore]
    public string Filters { get; set; }

    [DefaultValue( TextureFileFormat.DDS )]
    public TextureFileFormat OutputFileFormat { get; set; }

    [DefaultValue( NormalMapFormat.OpenGL )]
    public NormalMapFormat OutputNormalMapFormat { get; set; }

    [DefaultValue( true )]
    public bool RecalculateNormalMapZChannel { get; set; }

    [DefaultValue( false )]
    public bool ExportAllMips { get; set; }

    [DefaultValue( true )]
    public bool OverwriteExisting { get; set; }

    [DefaultValue( true )]
    public bool ExportTextureDefinition { get; set; }

    #endregion

    #region Constructor

    public TextureExportOptionsModel()
    {
    }

    [DeepCopyConstructor]
    public TextureExportOptionsModel( TextureExportOptionsModel source )
    {
    }

    #endregion

  }

}
