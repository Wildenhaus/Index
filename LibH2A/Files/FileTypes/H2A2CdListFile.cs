using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERh2a2_cd_list" )]
  [FileExtension( ".h2a2_cd_list" )]
  public class H2A2CdListFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "H2A2 CD List (.h2a2_cd_list)";

    #endregion

    #region Constructor

    public H2A2CdListFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
