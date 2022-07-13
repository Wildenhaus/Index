using System.ComponentModel;
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
          RemoveLODs = true,
          RemoveVolumes = true,
          OverwriteExisting = false
        };
      }
    }

    #endregion

    #region Properties

    [JsonIgnore]
    public string OutputPath { get; set; }

    [JsonIgnore]
    public string Filters { get; set; }

    [DefaultValue( ModelFileFormat.FBX )]
    public ModelFileFormat OutputFileFormat { get; set; }

    [DefaultValue( true )]
    public bool ExportTextures { get; set; }

    [DefaultValue( true )]
    public bool RemoveLODs { get; set; }

    [DefaultValue( true )]
    public bool RemoveVolumes { get; set; }

    [DefaultValue( true )]
    public bool OverwriteExisting { get; set; }

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
