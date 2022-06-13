using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERcdt" )]
  [FileExtension( ".cdt" )]
  public class CdtFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "CDT (.cdt)";

    #endregion

    #region Constructor

    public CdtFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
