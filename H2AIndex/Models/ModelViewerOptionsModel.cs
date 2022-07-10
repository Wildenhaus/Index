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
          RenderFxaaQuality = FXAALevel.Low
        };
      }
    }

    public bool ShowFps { get; set; }
    public bool ShowModelInfo { get; set; }
    public FXAALevel RenderFxaaQuality { get; set; }

    #endregion

  }

}
