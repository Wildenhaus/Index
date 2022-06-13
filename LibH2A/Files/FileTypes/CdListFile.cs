using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERcd_list" )]
  [FileExtension( ".cd_list" )]
  public class CdListFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "CD List (.cd_list)";

    #endregion

    #region Constructor

    public CdListFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
