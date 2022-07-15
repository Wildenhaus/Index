using DeepCopy;
using H2AIndex.Common;

namespace H2AIndex.Models
{

  public class TextureViewerOptionsModel : ObservableObject
  {

    #region Properties

    public static TextureViewerOptionsModel Default
    {
      get
      {
        return new TextureViewerOptionsModel
        {
          PreviewQuality = 1.0f
        };
      }
    }

    public float PreviewQuality { get; set; }

    #endregion

    #region Constructor

    public TextureViewerOptionsModel()
    {
    }

    [DeepCopyConstructor]
    public TextureViewerOptionsModel( TextureViewerOptionsModel source )
    {
    }

    #endregion

  }

}
