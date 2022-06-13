using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileExtension( ".ssl" )]
  public class SslFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Script (.ssl)";

    #endregion

    #region Constructor

    public SslFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
