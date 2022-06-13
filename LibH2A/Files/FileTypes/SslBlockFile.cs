using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERssl_block" )]
  [FileExtension( ".ssl_block" )]
  public class SslBlockFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Scripting Block (.ssl_block)";

    #endregion

    #region Constructor

    public SslBlockFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
