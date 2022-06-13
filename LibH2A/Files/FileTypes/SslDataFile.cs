using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERssl_data" )]
  [FileExtension( ".ssl_data" )]
  public class SslDataFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Scripting Data (.ssl_data)";

    #endregion

    #region Constructor

    public SslDataFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
