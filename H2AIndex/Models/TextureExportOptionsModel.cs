using System.Text.Json.Serialization;
using DeepCopy;
using H2AIndex.Common;
using H2AIndex.Common.Enumerations;

namespace H2AIndex.Models
{

  public class TextureExportOptionsModel : ObservableObject, IDeepCopy<TextureExportOptionsModel>
  {

    #region Data Members

    public static readonly TextureExportOptionsModel Default = new TextureExportOptionsModel
    {
      OutputFileFormat = TextureFileFormat.DDS,
      OutputNormalMapFormat = NormalMapFormat.OpenGL,
      RecalculateNormalMapZChannel = true,
      ExportAllMips = false,
      OverwriteExisting = false
    };

    #endregion

    #region Properties

    [JsonIgnore]
    public string OutputPath
    {
      get;
      set;
    }

    [JsonIgnore]
    public string Filters { get; set; }

    public TextureFileFormat OutputFileFormat { get; set; }
    public NormalMapFormat OutputNormalMapFormat { get; set; }
    public bool RecalculateNormalMapZChannel { get; set; }
    public bool ExportAllMips { get; set; }
    public bool OverwriteExisting { get; set; }

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
