using DeepCopy;
using H2AIndex.Models;

namespace H2AIndex.Common
{

  public interface IDeepCopy<T>
  {
  }

  public static class DeepCopyExtensions
  {

    [DeepCopyExtension]
    public static ModelExportOptionsModel DeepCopy( this ModelExportOptionsModel source )
      => source;

    [DeepCopyExtension]
    public static ModelViewerOptionsModel DeepCopy( this ModelViewerOptionsModel source )
      => source;

    [DeepCopyExtension]
    public static TextureExportOptionsModel DeepCopy( this TextureExportOptionsModel source )
      => source;

    [DeepCopyExtension]
    public static TextureViewerOptionsModel DeepCopy( this TextureViewerOptionsModel source )
      => source;

  }

}
