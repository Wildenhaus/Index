using System.Text.Json.Serialization;
using DeepCopy;
using H2AIndex.Common;
using H2AIndex.Common.Enumerations;

namespace H2AIndex.Models
{

  public class ModelExportOptionsModel : ObservableObject, IDeepCopy<ModelExportOptionsModel>
  {

    #region Data Members

    public static ModelExportOptionsModel Default
    {
      get
      {
        return new ModelExportOptionsModel
        {
          OutputFileFormat = ModelFileFormat.FBX,
          ExportTextures = true,
          ExportTextureDefinitions = true,
          RemoveLODs = false,
          RemoveVolumes = false,
          OverwriteExisting = false,

          OutputTextureFormat = TextureFileFormat.DDS,
          OutputNormalMapFormat = NormalMapFormat.OpenGL,
          RecalculateZChannel = true
        };
      }
    }

    #endregion

    #region Properties

    [JsonIgnore]
    public string OutputPath { get; set; }

    [JsonIgnore]
    public string Filters { get; set; }

    public ModelFileFormat OutputFileFormat { get; set; }
    public bool ExportTextures { get; set; }
    public bool ExportTextureDefinitions { get; set; }
    public bool RemoveLODs { get; set; }
    public bool RemoveVolumes { get; set; }
    public bool OverwriteExisting { get; set; }

    public TextureFileFormat OutputTextureFormat { get; set; }
    public NormalMapFormat OutputNormalMapFormat { get; set; }
    public bool RecalculateZChannel { get; set; }

    #endregion

    #region Constructor

    public ModelExportOptionsModel()
    {
    }

    [DeepCopyConstructor]
    public ModelExportOptionsModel( ModelExportOptionsModel source )
    {
    }

    #endregion

  }

}
