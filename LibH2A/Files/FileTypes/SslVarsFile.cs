using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileExtension( ".ssl_vars" )]
  public class SslVarsFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Scripting Variables (.ssl_vars)";

    #endregion

    #region Constructor

    public SslVarsFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
