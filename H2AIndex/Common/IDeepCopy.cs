using DeepCopy;

namespace H2AIndex.Common
{

  public interface IDeepCopy<T>
  {
  }

  public static class IDeepCopyExtensions
  {

    [DeepCopyExtension]
    public static T DeepCopy<T>( this T source )
      where T : IDeepCopy<T>
      => source;

  }

}
