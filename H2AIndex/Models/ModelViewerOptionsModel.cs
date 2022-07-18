using System.ComponentModel;
using DeepCopy;
using H2AIndex.Common;
using HelixToolkit.SharpDX.Core;

namespace H2AIndex.Models
{

  public class ModelViewerOptionsModel : ObservableObject
  {

    #region Properties

    public static ModelViewerOptionsModel Default
    {
      get
      {
        return new ModelViewerOptionsModel
        {
          ShowFps = true,
          ShowModelInfo = true,
          DefaultToFlycam = false,
          DefaultHideLODs = false,
          DefaultHideVolumes = true,
          RenderFxaaQuality = FXAALevel.Low,
          ModelTexturePreviewQuality = 1f
        };
      }
    }

    [DefaultValue( true )]
    public bool ShowFps { get; set; }

    [DefaultValue( true )]
    public bool ShowModelInfo { get; set; }

    [DefaultValue( false )]
    public bool DefaultToFlycam { get; set; }

    [DefaultValue( false )]
    public bool DefaultHideLODs { get; set; }

    [DefaultValue( true )]
    public bool DefaultHideVolumes { get; set; }

    [DefaultValue( FXAALevel.Low )]
    public FXAALevel RenderFxaaQuality { get; set; }

    [DefaultValue( 1.0f )]
    public float ModelTexturePreviewQuality { get; set; }

    #endregion

    #region Constructor

    public ModelViewerOptionsModel()
    {
    }

    [DeepCopyConstructor]
    public ModelViewerOptionsModel( ModelViewerOptionsModel source )
    {
    }

    #endregion

  }

}
