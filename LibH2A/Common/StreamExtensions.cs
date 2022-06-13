using System.IO;

namespace Saber3D.Common
{

  public static class StreamExtensions
  {


    public static Stream ToBufferedStream( this Stream stream )
      => new BufferedStream( stream );

    public static Stream ToBufferedStream( this Stream stream, int bufferSize )
      => new BufferedStream( stream, bufferSize );

  }

}
